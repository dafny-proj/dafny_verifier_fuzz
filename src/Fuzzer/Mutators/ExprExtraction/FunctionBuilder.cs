using NF = AST.NodeFactory;

namespace Fuzzer;

public class FunctionData {
  public Expression E;
  public Dictionary<Expression, Formal> Params;
  public List<Expression> Requires;
  public List<Expression> Reads;
  public bool Unknown;

  public FunctionData(Expression e, bool unknown = false,
  Dictionary<Expression, Formal>? params_ = null,
  IEnumerable<Expression>? req = null, IEnumerable<Expression>? reads = null) {
    E = e;
    Unknown = unknown;
    Params = params_ != null ? new(params_) : new();
    Requires = req != null ? new(req) : new();
    Reads = reads != null ? new(reads) : new();
  }

  public void AddRequires(Expression e) => Requires.Add(e);
  public void AddReads(Expression e) => Reads.Add(e);
  public bool IsSpecFree() => Requires.Count == 0 && Reads.Count == 0;
}

public class FunctionBuilder {
  public IRandomizer Rand;
  public IGenerator Gen;
  public ClassDecl? ThisClass;
  public Expression? ThisObject;
  private int guardDepth = 0;

  public FunctionBuilder(IRandomizer rand, IGenerator gen) {
    Rand = rand;
    Gen = gen;
  }

  public FunctionData BuildFromExpression(Expression e) {
    return VisitExpr(e);
  }

  private FunctionData BuiltIn(Expression e) {
    return new FunctionData(e: e);
  }
  private FunctionData Identity(Expression e) {
    var param = new Formal(name: Gen.GenFormalName(), type: e.Type);
    return new FunctionData(
      e: new IdentifierExpr(param),
      params_: new() { { e, param } });
  }
  private FunctionData Compose(Expression e, IEnumerable<FunctionData> fds) {
    return new FunctionData(e: e,
      params_:
        fds.SelectMany(f => f.Params).ToDictionary(p => p.Key, p => p.Value),
      req: fds.SelectMany(f => f.Requires),
      reads: fds.SelectMany(f => f.Reads));
  }
  // Only use for propagating information between levels of expressions. The
  // contained expression cannot be validly used in the function extracted. 
  private FunctionData Intermediate(Expression e) {
    return new FunctionData(e: e, unknown: true);
  }
  private bool TryUseExprAsThis(Expression e) {
    if (ThisObject != null) { return false; }
    var t = e.Type;
    if (t is UserDefinedType ut && ut.TypeDecl is ClassDecl cd) {
      // Disallow adding members to built-in types.
      if (cd is not (ArrayClassDecl or ArrowTypeDecl)) {
        ThisObject = e;
        ThisClass = cd;
        return true;
      }
    }
    return false;
  }
  private Expression GenDatatypeConstructorCheck(Expression dtv,
  IEnumerable<DatatypeConstructorDecl> constructors) {
    Contract.Requires(constructors.Count() > 0);
    Expression? allChecks = null;
    foreach (var c in constructors) {
      var thisCheck = NF.CreateDatatypeConstructorCheck(
        Cloner.Clone<Expression>(dtv), c);
      allChecks = allChecks == null ? thisCheck
        : NF.CreateOrExpr(allChecks, thisCheck);
    }
    return allChecks!;
  }

  private FunctionData VisitExpr(Expression e_) {
    return e_ switch {
      LiteralExpr e => VisitLiteralExpr(e),
      BinaryExpr e => VisitBinaryExpr(e),
      DatatypeUpdateExpr e => VisitDatatypeUpdateExpr(e),
      MemberSelectExpr e => VisitMemberSelectExpr(e),
      ITEExpr e => VisitITEExpr(e),
      CollectionElementExpr e => VisitCollectionElementExpr(e),
      _ => Identity(e_),
    };
  }

  private FunctionData VisitLiteralExpr(LiteralExpr e) {
    // A literal can be passed in by parameter or built into the function.
    return Rand.RandBool() ? Identity(e) : BuiltIn(e);
  }

  private FunctionData VisitBinaryExpr(BinaryExpr e) {
    if (Rand.RandBool()) {
      return Identity(e);
    } else {
      // Construct the binary expression from its subexpressions. The parameters 
      // are the combination of parameters required by its subexpressions.
      var sub0 = VisitExpr(e.E0);
      var sub1 = VisitExpr(e.E1);
      return Compose(
        e: new BinaryExpr(e.Op, sub0.E, sub1.E), fds: new[] { sub0, sub1 });
    }
  }

  private FunctionData VisitDatatypeUpdateExpr(DatatypeUpdateExpr e) {
    if (Rand.RandBool()) {
      return Identity(e);
    } else {
      // Compose function from subexpressions.
      var fds = new List<FunctionData>();
      var fd = VisitExpr(e.DatatypeValue);
      fds.Add(fd);
      var dtv = fd.E;
      var updates = new List<DatatypeUpdatePair>();
      foreach (var u in e.Updates) {
        fd = VisitExpr(u.Value);
        fds.Add(fd);
        updates.Add(new DatatypeUpdatePair(u.Key, fd.E));
      }
      var f = Compose(e: new DatatypeUpdateExpr(dtv, updates), fds: fds);
      // Find the constructors which match the updated fields. The datatype 
      // value updated must match one of the constructors.
      var constructors = e.Updates[0].Key.Constructors.AsEnumerable();
      foreach (var u in e.Updates) {
        constructors = constructors.Intersect(u.Key.Constructors);
      }
      f.AddRequires(GenDatatypeConstructorCheck(dtv, constructors));
      return f;
    }
  }

  private FunctionData VisitMemberSelectExpr(MemberSelectExpr e) {
    if (e.Member is DatatypeDestructorDecl) { return VisitDatatypeDestructor(e); }
    if (e.Member is FieldDecl) { return VisitField(e); }
    // TODO: Handle other cases.
    return Identity(e);
  }

  private FunctionData VisitDatatypeDestructor(MemberSelectExpr e) {
    Contract.Requires(e.Member is DatatypeDestructorDecl);
    if (guardDepth != 0) {
      // Case 1: This expression happened in a conditional, it is unclear how
      // to generate the precondition to guard this expression. It is also
      // potentially unsafe and cannot be used on its own as a parameter.
      // Delegate to the parent.
      // TODO: Alternatively, replace `e` with `assume Safe(e); e`.
      return Intermediate(e);
    }
    // If the expression was not guarded, we know that it must be safe. We can
    // use it (Case 2) directly as an argument, or (Case 3) generate the 
    // precondition as the safety condition of this expression.
    // Case 2:
    if (Rand.RandBool()) { return Identity(e); }
    // Case 3: 
    var receiverFD = VisitExpr(e.Receiver);
    Contract.Assert(!receiverFD.Unknown);
    var receiver = receiverFD.E;
    var f = Compose(
      e: new MemberSelectExpr(receiver, e.Member), fds: new[] { receiverFD });
    f.AddRequires(GenDatatypeConstructorCheck(receiver,
      ((DatatypeDestructorDecl)e.Member).Constructors));
    return f;
  }

  private FunctionData VisitField(MemberSelectExpr e) {
    Contract.Requires(e.Member is FieldDecl);
    if (Rand.RandBool()) { return Identity(e); }
    if (e.Receiver is StaticReceiverExpr) { return BuiltIn(e); }
    var fld = (FieldDecl)e.Member;
    FunctionData f;
    Expression receiver;
    // Potentially introduce an instance function if we find an object.
    if (Rand.RandBool() && TryUseExprAsThis(e.Receiver)) {
      receiver = new ThisExpr(e.Receiver.Type);
      f = BuiltIn(new MemberSelectExpr(receiver, e.Member));
    } else {
      var receiverFD = VisitExpr(e.Receiver);
      receiver = receiverFD.E;
      f = Compose(e: new MemberSelectExpr(receiver, e.Member),
        fds: new[] { receiverFD });
    }
    if (!fld.IsBuiltIn) {
      // Add reads clause if reading a non-static mutable user-defined field.
      // TODO: Fields currently don't have static/const attributes.
      f.AddReads(Cloner.Clone<Expression>(receiver));
    }
    return f;
  }

  private FunctionData VisitITEExpr(ITEExpr e) {
    if (Rand.RandBool()) { return Identity(e); }
    var guard = VisitExpr(e.Guard);
    guardDepth++;
    var thn = VisitExpr(e.Thn);
    var els = VisitExpr(e.Els);
    guardDepth--;
    if (guard.Unknown || thn.Unknown || els.Unknown) {
      return guardDepth == 0 ? Identity(e) : Intermediate(e);
    }
    return Compose(new ITEExpr(guard.E, thn.E, els.E, type: e.Type),
      new[] { guard, thn, els });
  }

  // 0 <= index && index < length
  private Expression GenBoundsCheck(Expression index, Expression length) {
    return NF.CreateAndExpr(
      e0: NF.CreateLEExpr(
        NF.CreateIntLiteral(0), Cloner.Clone<Expression>(index)),
      e1: NF.CreateLTExpr(
        Cloner.Clone<Expression>(index), Cloner.Clone<Expression>(length)));
  }

  private FunctionData VisitCollectionElementExpr(CollectionElementExpr e) {
    // Case 1: Cannot be used on its own.
    if (guardDepth != 0 && e.Collection.Type is not MultiSetType) {
      return Intermediate(e);
    }
    // Case 2: Pass in entire expression by value.
    if (Rand.RandBool()) { return Identity(e); }
    // Case 3: Decompose expression.
    var collection = VisitExpr(e.Collection);
    var index = VisitExpr(e.Index);
    Contract.Assert(!collection.Unknown && !index.Unknown);
    var f = Compose(e: new CollectionElementExpr(collection.E, index.E, e.Type),
      fds: new[] { collection, index });
    // Add requires and reads.
    var collectionType = e.Collection.Type;
    if (collectionType is ArrayType at) {
      // read: array
      f.AddReads(collection.E);
      var arrDec = (ArrayClassDecl)at.TypeDecl;
      // TODO: Handle multidimensional array indexing.
      // req: 0 <= index < array.Length
      var arrLength = new MemberSelectExpr(
        receiver: Cloner.Clone<Expression>(collection.E),
        member: arrDec.LengthField());
      f.AddRequires(GenBoundsCheck(index.E, arrLength));
    } else if (collectionType is MapType) {
      // req: index in map
      f.AddRequires(NF.CreateInExpr(
        e0: Cloner.Clone<Expression>(index.E),
        e1: Cloner.Clone<Expression>(collection.E)));
    } else if (collectionType is SeqType) {
      // req: 0 <= index < |seq|
      var seqLength = NF.CreateCardinalityExpr(collection.E);
      f.AddRequires(GenBoundsCheck(index.E, seqLength));
    } else if (collectionType is MultiSetType) {
      // No precondition/reads required.
    } else {
      throw new InvalidASTOperationException(
        $"Collection of `{e}` should be of array, map, seq, multiset type.  Got `{collectionType}`.");
    }
    return f;
  }

}
