namespace Fuzzer;

public class FunctionData {
  public Expression E;
  public Dictionary<Expression, Formal> Params;
  public List<Expression> Requires;
  public List<Expression> Reads;

  public FunctionData(Expression e,
  Dictionary<Expression, Formal>? params_ = null,
  IEnumerable<Expression>? req = null, IEnumerable<Expression>? reads = null) {
    E = e;
    Params = params_ != null ? new(params_) : new();
    Requires = req != null ? new(req) : new();
    Reads = reads != null ? new(reads) : new();
  }

  public void AddRequires(Expression e) => Requires.Add(e);
  public void AddReads(Expression e) => Reads.Add(e);
}

public class FunctionBuilder {
  public IRandomizer Rand;
  public IGenerator Gen;
  public ClassDecl? ThisClass;
  public Expression? ThisObject;

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

  private FunctionData VisitExpr(Expression e_) {
    return e_ switch {
      LiteralExpr e => VisitLiteralExpr(e),
      BinaryExpr e => VisitBinaryExpr(e),
      DatatypeUpdateExpr e => VisitDatatypeUpdateExpr(e),
      MemberSelectExpr e => VisitMemberSelectExpr(e),
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
      Expression? requires = null;
      var constructors = e.Updates[0].Key.Constructors.AsEnumerable();
      foreach (var u in e.Updates) {
        constructors = constructors.Intersect(u.Key.Constructors);
      }
      foreach (var c in constructors) {
        var constructorCheck
          = NodeFactory.CreateDatatypeConstructorCheck(
            Cloner.Clone<Expression>(dtv), c);
        requires = requires == null ? constructorCheck
          : NodeFactory.CreateOrExpr(requires, constructorCheck);
      }
      f.AddRequires(requires!);
      return f;
    }
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

  private FunctionData VisitMemberSelectExpr(MemberSelectExpr e) {
    if (Rand.RandBool()) {
      return Identity(e);
    } else {
      // Compose function from subexpressions.
      FunctionData f;
      if (e.Receiver is (StaticReceiverExpr)) {
        f = BuiltIn(e);
      } else {
        Expression receiver;
        if (Rand.RandBool() && TryUseExprAsThis(e.Receiver)) {
          receiver = new ThisExpr(e.Type);
          f = BuiltIn(new MemberSelectExpr(receiver, e.Member));
        } else {
          var receiverFD = VisitExpr(e.Receiver);
          receiver = receiverFD.E;
          f = Compose(e: new MemberSelectExpr(receiver, e.Member),
            fds: new[] { receiverFD });
        }
        // Add reads clause if reading a non-static mutable user-defined field.
        if (e.Member is FieldDecl fld && !fld.IsBuiltIn) {
          // TODO: Fields currently don't have static/const attributes.
          f.AddReads(new FrameFieldExpr(Cloner.Clone<Expression>(receiver), fld));
        }
      }
      return f;
    }
  }
}