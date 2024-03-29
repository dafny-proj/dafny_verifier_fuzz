namespace AST.Cloner;

public partial class ASTCloner {
  private void SetType(Expression e, Expression c) {
    c.Type = CloneType(e.Type);
  }

  private Expression CloneExpression(Expression e) {
    return e switch {
      LiteralExpr le => CloneLiteralExpr(le),
      IdentifierExpr ie => CloneIdentifierExpr(ie),
      ParensExpr pe => CloneParensExpr(pe),
      BinaryExpr be => CloneBinaryExpr(be),
      UnaryExpr ue => CloneUnaryExpr(ue),
      MemberSelectExpr me => CloneMemberSelectExpr(me),
      CollectionElementExpr ce => CloneCollectionSelectExpr(ce),
      CollectionSliceExpr ce => CloneCollectionSelectExpr(ce),
      SeqDisplayExpr se => CloneCollectionDisplayExpr(se),
      SetDisplayExpr se => CloneCollectionDisplayExpr(se),
      MultiSetDisplayExpr me => CloneCollectionDisplayExpr(me),
      MapDisplayExpr me => CloneCollectionDisplayExpr(me),
      SetComprehensionExpr se => CloneComprehensionExpr(se),
      MapComprehensionExpr me => CloneComprehensionExpr(me),
      CollectionUpdateExpr ce => CloneCollectionUpdateExpr(ce),
      MultiSetConstructionExpr me => CloneMultiSetConstructionExpr(me),
      SeqConstructionExpr se => CloneSeqConstructionExpr(se),
      DatatypeValueExpr de => CloneDatatypeValueExpr(de),
      ThisExpr te => CloneThisExpr(te),
      AutoGeneratedExpr ae => CloneAutoGeneratedExpr(ae),
      WildcardExpr we => CloneWildcardExpr(we),
      FunctionCallExpr fe => CloneFunctionCallExpr(fe),
      StaticReceiverExpr se => CloneStaticReceiverExpr(se),
      ITEExpr ite => CloneITEExpr(ite),
      LetExpr le => CloneLetExpr(le),
      QuantifierExpr qe => CloneQuantifierExpr(qe),
      MatchExpr me => CloneMatchExpr(me),
      LambdaExpr le => CloneLambdaExpr(le),
      DatatypeUpdateExpr de => CloneDatatypeUpdateExpr(de),
      TypeUnaryExpr te => CloneTypeUnaryExpr(te),
      _ => throw new UnsupportedNodeCloningException(e),
    };
  }

  private LiteralExpr CloneLiteralExpr(LiteralExpr e) {
    return e switch {
      BoolLiteralExpr be => new BoolLiteralExpr(be.Value),
      CharLiteralExpr ce => new CharLiteralExpr(ce.Value),
      IntLiteralExpr ie => new IntLiteralExpr(ie.Value),
      RealLiteralExpr re => new RealLiteralExpr(re.Value),
      StringLiteralExpr se => new StringLiteralExpr(se.Value),
      NullLiteralExpr ne => new NullLiteralExpr(CloneType(ne.Type)),
      _ => throw new UnsupportedNodeCloningException(e),
    };
  }

  private IdentifierExpr CloneIdentifierExpr(IdentifierExpr e) {
    return new IdentifierExpr(CloneVariable(e.Var));
  }

  private ParensExpr CloneParensExpr(ParensExpr e) {
    return new ParensExpr(CloneExpression(e.E));
  }

  private BinaryExpr CloneBinaryExpr(BinaryExpr e) {
    return new BinaryExpr(e.Op, CloneExpression(e.E0), CloneExpression(e.E1));
  }

  private UnaryExpr CloneUnaryExpr(UnaryExpr e) {
    return new UnaryExpr(e.Op, CloneExpression(e.E));
  }

  private MemberSelectExpr CloneMemberSelectExpr(MemberSelectExpr e) {
    var receiver = CloneExpression(e.Receiver);
    var member = (MemberDecl)CloneDeclRef(e.Member);
    MemberSelectExpr c;
    if (e is FrameFieldExpr) {
      c = new FrameFieldExpr(receiver, (FieldDecl)member);
    } else {
      c = new MemberSelectExpr(receiver, member);
      SetType(e, c);
    }
    return c;
  }

  private CollectionElementExpr CloneCollectionSelectExpr(CollectionElementExpr e) {
    var c = new CollectionElementExpr(
      CloneExpression(e.Collection), CloneExpression(e.Index));
    SetType(e, c);
    return c;
  }

  private CollectionSliceExpr CloneCollectionSelectExpr(CollectionSliceExpr e) {
    var c = new CollectionSliceExpr(CloneExpression(e.Collection),
      e.Index0 == null ? null : CloneExpression(e.Index0),
      e.Index1 == null ? null : CloneExpression(e.Index1));
    SetType(e, c);
    return c;
  }

  private SeqDisplayExpr CloneCollectionDisplayExpr(SeqDisplayExpr e) {
    var c = new SeqDisplayExpr(e.Elements.Select(CloneExpression));
    SetType(e, c);
    return c;
  }

  private SetDisplayExpr CloneCollectionDisplayExpr(SetDisplayExpr e) {
    var c = new SetDisplayExpr(e.Elements.Select(CloneExpression));
    SetType(e, c);
    return c;
  }

  private MultiSetDisplayExpr CloneCollectionDisplayExpr(MultiSetDisplayExpr e) {
    var c = new MultiSetDisplayExpr(e.Elements.Select(CloneExpression));
    SetType(e, c);
    return c;
  }

  private MapDisplayExpr CloneCollectionDisplayExpr(MapDisplayExpr e) {
    var c = new MapDisplayExpr(e.Elements.Select(CloneExpressionPair));
    SetType(e, c);
    return c;
  }

  private SetComprehensionExpr CloneComprehensionExpr(SetComprehensionExpr e) {
    var c = new SetComprehensionExpr(CloneQuantifierDomain(e.QuantifierDomain),
      e.Value == null ? null : CloneExpression(e.Value));
    SetType(e, c);
    return c;
  }

  private MapComprehensionExpr CloneComprehensionExpr(MapComprehensionExpr e) {
    var c = new MapComprehensionExpr(CloneQuantifierDomain(e.QuantifierDomain),
      e.Key == null ? null : CloneExpression(e.Key), CloneExpression(e.Value));
    SetType(e, c);
    return c;
  }

  private CollectionUpdateExpr CloneCollectionUpdateExpr(CollectionUpdateExpr e) {
    return new CollectionUpdateExpr(CloneExpression(e.Collection),
      CloneExpression(e.Index), CloneExpression(e.Value));
  }

  private MultiSetConstructionExpr CloneMultiSetConstructionExpr(MultiSetConstructionExpr e) {
    var c = new MultiSetConstructionExpr(CloneExpression(e.E));
    SetType(e, c);
    return c;
  }

  private SeqConstructionExpr CloneSeqConstructionExpr(SeqConstructionExpr e) {
    var c = new SeqConstructionExpr(
      CloneExpression(e.Count), CloneExpression(e.Initialiser));
    SetType(e, c);
    return c;
  }

  private DatatypeValueExpr CloneDatatypeValueExpr(DatatypeValueExpr e) {
    var c = new DatatypeValueExpr(
      (DatatypeConstructorDecl)CloneDeclRef(e.Constructor),
      e.ConstructorArguments.Select(CloneExpression));
    SetType(e, c);
    return c;
  }

  private ThisExpr CloneThisExpr(ThisExpr e) {
    var type = CloneType(e.Type);
    if (e is ImplicitThisExpr) {
      return new ImplicitThisExpr(type);
    }
    return new ThisExpr(type);
  }

  private AutoGeneratedExpr CloneAutoGeneratedExpr(AutoGeneratedExpr e) {
    return new AutoGeneratedExpr(CloneExpression(e.E));
  }

  private WildcardExpr CloneWildcardExpr(WildcardExpr e) {
    return new WildcardExpr();
  }

  private FunctionCallExpr CloneFunctionCallExpr(FunctionCallExpr e) {
    return new FunctionCallExpr(
      CloneExpression(e.Callee), e.Arguments.Select(CloneExpression));
  }

  private StaticReceiverExpr CloneStaticReceiverExpr(StaticReceiverExpr e) {
    var d = (TopLevelDecl)CloneDeclRef(e.Decl);
    if (e is ImplicitStaticReceiverExpr) {
      return new ImplicitStaticReceiverExpr(d);
    }
    return new StaticReceiverExpr(d);
  }

  private ITEExpr CloneITEExpr(ITEExpr e) {
    var c = new ITEExpr(CloneExpression(e.Guard),
      CloneExpression(e.Thn), CloneExpression(e.Els));
    SetType(e, c);
    return c;
  }

  private LetExpr CloneLetExpr(LetExpr e) {
    var c = new LetExpr(e.Vars.Select(v =>
      new VarExpressionPair(CloneBoundVar(v.Key), CloneExpression(v.Value))),
      CloneExpression(e.Body));
    SetType(e, c);
    return c;
  }

  private QuantifierExpr CloneQuantifierExpr(QuantifierExpr e) {
    var qd = CloneQuantifierDomain(e.QuantifierDomain);
    var term = CloneExpression(e.Term);
    if (e is ForallExpr) {
      return new ForallExpr(qd, term);
    } else if (e is ExistsExpr) {
      return new ExistsExpr(qd, term);
    } else {
      throw new UnsupportedNodeCloningException(e);
    }
  }

  private MatchExpr CloneMatchExpr(MatchExpr e) {
    var c = new MatchExpr(
      CloneExpression(e.Selector), e.Cases.Select(CloneMatchExprCase));
    SetType(e, c);
    return c;
  }

  private LambdaExpr CloneLambdaExpr(LambdaExpr e) {
    var c = new LambdaExpr(
      params_: e.Params.Select(CloneBoundVar),
      result: CloneExpression(e.Result),
      pre: CloneSpecification(e.Precondition),
      reads: CloneSpecification(e.ReadFrame));
    SetType(e, c);
    return c;
  }

  private DatatypeUpdateExpr CloneDatatypeUpdateExpr(DatatypeUpdateExpr e) {
    return new DatatypeUpdateExpr(CloneExpression(e.DatatypeValue),
      e.Updates.Select(CloneDatatypeUpdatePair));
  }

  private TypeUnaryExpr CloneTypeUnaryExpr(TypeUnaryExpr e) {
    var e_ = CloneExpression(e.E);
    var t = CloneType(e.T);
    TypeUnaryExpr c = e switch {
      TypeConversionExpr => new TypeConversionExpr(e_, t),
      TypeCheckExpr => new TypeCheckExpr(e_, t),
      _ => throw new UnsupportedNodeCloningException(e),
    };
    SetType(e, c);
    return c;
  }

}
