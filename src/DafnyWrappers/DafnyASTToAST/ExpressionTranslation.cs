using System.Numerics;
using System.Diagnostics.Contracts;
using Dafny = Microsoft.Dafny;

namespace AST.Translation;

public partial class ASTTranslator {
  private Expression TranslateExpression(Dafny.Node n) {
    // For now, ignore differences between attributed/frame expressions 
    // and expressions.
    return n switch {
      Dafny.AttributedExpression ae => TranslateExpression(ae.E),
      Dafny.FrameExpression fe => TranslateExpression(fe.E),
      Dafny.Expression e => TranslateExpression(e),
      _ => throw new UnsupportedTranslationException(n),
    };
  }

  private Expression TranslateExpression(Dafny.Expression e) {
    return e switch {
      Dafny.LiteralExpr le => TranslateLiteralExpr(le),
      Dafny.IdentifierExpr ie => TranslateIdentifierExpr(ie),
      Dafny.BinaryExpr be => TranslateBinaryExpr(be),
      Dafny.UnaryOpExpr ue => TranslateUnaryExpr(ue),
      Dafny.MemberSelectExpr mse => TranslateMemberSelectExpr(mse),
      Dafny.SeqSelectExpr sse => TranslateCollectionSelectExpr(sse),
      Dafny.DisplayExpression de => TranslateDisplayExpr(de),
      Dafny.MapDisplayExpr mde => TranslateMapDisplayExpr(mde),
      Dafny.SeqUpdateExpr sue => TranslateCollectionUpdateExpr(sue),
      Dafny.DatatypeValue dv => TranslateDatatypeValue(dv),
      Dafny.ThisExpr te => TranslateThisExpr(te),
      Dafny.WildcardExpr we => TranslateWildcardExpr(we),
      Dafny.FunctionCallExpr fe => TranslateFunctionCallExpr(fe),
      Dafny.ITEExpr ie => TranslateITEExpr(ie),
      Dafny.LetExpr le => TranslateLetExpr(le),
      Dafny.QuantifierExpr qe => TranslateQuantifierExpr(qe),
      Dafny.NestedMatchExpr me => TranslateMatchExpr(me),
      Dafny.MultiSetFormingExpr me => TranslateMultiSetFormingExpr(me),
      Dafny.ApplyExpr ae => TranslateFunctionCallExpr(ae),
      Dafny.LambdaExpr le => TranslateLambdaExpr(le),
      Dafny.ConcreteSyntaxExpression cse => cse switch {
        Dafny.AutoGeneratedExpression ae => TranslateAutoGeneratedExpr(ae),
        Dafny.ParensExpression pe => TranslateParensExpr(pe),
        Dafny.DatatypeUpdateExpr de => TranslateDatatypeUpdateExpr(de),
        _ => TranslateExpression(cse.ResolvedExpression),
      },
      _ => throw new UnsupportedTranslationException(e),
    };
  }

  private Expression TranslateLiteralExpr(Dafny.LiteralExpr le) {
    if (le is Dafny.StaticReceiverExpr sre) {
      return TranslateStaticReceiverExpr(sre);
    } else if (le.Value == null) {
      return new NullLiteralExpr(TranslateType(le.Type));
    } else if (le.Value is bool b) {
      return new BoolLiteralExpr(b);
    } else if (le is Dafny.CharLiteralExpr ce) {
      return new CharLiteralExpr((string)ce.Value);
    } else if (le.Value is BigInteger i) {
      return new IntLiteralExpr(i);
    } else if (le.Value is Microsoft.BaseTypes.BigDec f) {
      return new RealLiteralExpr(f.ToDecimalString());
    } else if (le is Dafny.StringLiteralExpr se) {
      return new StringLiteralExpr((string)se.Value);
    } else {
      throw new UnsupportedTranslationException(le);
    }
  }

  private IdentifierExpr TranslateIdentifierExpr(Dafny.IdentifierExpr ie) {
    return new IdentifierExpr(TranslateVariable(ie.Var));
  }

  private ParensExpr TranslateParensExpr(Dafny.ParensExpression pe) {
    return new ParensExpr(TranslateExpression(pe.E));
  }

  private AutoGeneratedExpr TranslateAutoGeneratedExpr(Dafny.AutoGeneratedExpression ae) {
    return new AutoGeneratedExpr(TranslateExpression(ae.E));
  }

  private MemberSelectExpr TranslateMemberSelectExpr(Dafny.MemberSelectExpr mse) {
    return new MemberSelectExpr(
      TranslateExpression(mse.Obj), (MemberDecl)TranslateDeclRef(mse.Member));
  }

  private CollectionSelectExpr TranslateCollectionSelectExpr(Dafny.SeqSelectExpr sse) {
    var collection = TranslateExpression(sse.Seq);
    if (sse.SelectOne) {
      return new CollectionElementExpr(collection, TranslateExpression(sse.E0));
    } else {
      return new CollectionSliceExpr(collection,
        sse.E0 == null ? null : TranslateExpression(sse.E0),
        sse.E1 == null ? null : TranslateExpression(sse.E1));
    }
  }

  private CollectionDisplayExpr<Expression>
  TranslateDisplayExpr(Dafny.DisplayExpression de) {
    var elements = de.Elements.Select(TranslateExpression);
    return de switch {
      Dafny.SeqDisplayExpr => new SeqDisplayExpr(elements),
      Dafny.SetDisplayExpr => new SetDisplayExpr(elements),
      Dafny.MultiSetDisplayExpr => new MultiSetDisplayExpr(elements),
      _ => throw new UnsupportedTranslationException(de),
    };
  }

  private MapDisplayExpr TranslateMapDisplayExpr(Dafny.MapDisplayExpr mde) {
    return new MapDisplayExpr(mde.Elements.Select(TranslateExpressionPair));
  }

  private CollectionUpdateExpr
  TranslateCollectionUpdateExpr(Dafny.SeqUpdateExpr sue) {
    return new CollectionUpdateExpr(
      collection: TranslateExpression(sue.Seq),
      index: TranslateExpression(sue.Index),
      value: TranslateExpression(sue.Value));
  }

  private DatatypeValueExpr TranslateDatatypeValue(Dafny.DatatypeValue dv) {
    return new DatatypeValueExpr(
      (DatatypeConstructorDecl)TranslateDeclRef(dv.Ctor),
      dv.Arguments.Select(TranslateExpression));
  }

  private ThisExpr TranslateThisExpr(Dafny.ThisExpr te) {
    var type = TranslateType(te.Type);
    if (te is Dafny.ImplicitThisExpr) {
      return new ImplicitThisExpr(type);
    }
    return new ThisExpr(type);
  }

  private WildcardExpr TranslateWildcardExpr(Dafny.WildcardExpr we) {
    return new WildcardExpr();
  }

  private FunctionCallExpr TranslateFunctionCallExpr(Dafny.FunctionCallExpr fe) {
    return new FunctionCallExpr(
      callee: new MemberSelectExpr(
        TranslateExpression(fe.Receiver),
        (FunctionDecl)TranslateDeclRef(fe.Function)),
      arguments: fe.Args.Select(TranslateExpression));
  }

  private FunctionCallExpr TranslateFunctionCallExpr(Dafny.ApplyExpr ae) {
    return new FunctionCallExpr(
      callee: TranslateExpression(ae.Function),
      arguments: ae.Args.Select(TranslateExpression));
  }

  private StaticReceiverExpr TranslateStaticReceiverExpr(Dafny.StaticReceiverExpr se) {
    Contract.Assert(se.Type is Dafny.UserDefinedType);
    var d = (TopLevelDecl)TranslateDeclRef(((Dafny.UserDefinedType)se.Type).ResolvedClass);
    if (se.IsImplicit) {
      return new ImplicitStaticReceiverExpr(d);
    }
    return new StaticReceiverExpr(d);
  }

  private ITEExpr TranslateITEExpr(Dafny.ITEExpr ie) {
    return new ITEExpr(TranslateExpression(ie.Test),
      TranslateExpression(ie.Thn), TranslateExpression(ie.Els));
  }

  private LetExpr TranslateLetExpr(Dafny.LetExpr le) {
    var vars = new List<KeyValuePair<BoundVar, Expression>>();
    foreach (var (l, r) in le.BoundVars.Zip(le.RHSs)) {
      vars.Add(new KeyValuePair<BoundVar, Expression>(
        TranslateBoundVar(l), TranslateExpression(r)));
    }
    return new LetExpr(vars, TranslateExpression(le.Body));
  }

  private QuantifierExpr TranslateQuantifierExpr(Dafny.QuantifierExpr qe) {
    var qd = TranslateQuantifierDomain(qe.BoundVars, qe.Range);
    var term = TranslateExpression(qe.Term);
    if (qe is Dafny.ForallExpr) {
      return new ForallExpr(qd, term);
    } else if (qe is Dafny.ExistsExpr) {
      return new ExistsExpr(qd, term);
    } else {
      throw new UnsupportedTranslationException(qe);
    }
  }

  private MatchExpr TranslateMatchExpr(Dafny.NestedMatchExpr me) {
    var selector = TranslateExpression(me.Source);
    var cases = new List<MatchExprCase>();
    foreach (var c in me.Cases) {
      var body = TranslateExpression(c.Body);
      var matcher = TranslateMatcher(c.Pat);
      cases.Add(new MatchExprCase(matcher, body));
    }
    return new MatchExpr(selector, cases);
  }

  private MultiSetFormingExpr TranslateMultiSetFormingExpr(Dafny.MultiSetFormingExpr me) {
    return new MultiSetFormingExpr(TranslateExpression(me.E));
  }

  private LambdaExpr TranslateLambdaExpr(Dafny.LambdaExpr le) {
    var params_ = le.BoundVars.Select(TranslateBoundVar);
    var result = TranslateExpression(le.Body);
    var pre = le.Range == null ? null :
      new Specification(Specification.Type.Precondition,
      new[] { TranslateExpression(le.Range) });
    var reads = TranslateSpecification(Specification.Type.ReadFrame, le.Reads);
    return new LambdaExpr(params_, result, pre, reads);
  }

  private DatatypeUpdateExpr TranslateDatatypeUpdateExpr(Dafny.DatatypeUpdateExpr de) {
    Contract.Assert(de.Updates.Count == de.Members.Count());
    Contract.Assert(de.Members.TrueForAll(m => m is Dafny.DatatypeDestructor));
    var datatypeValue = TranslateExpression(de.Root);
    var updates = new List<DatatypeUpdatePair>(de.Updates.Count);
    foreach (var (m, u) in de.Members.Zip(de.Updates)) {
      var id = (DatatypeDestructorDecl)TranslateDeclRef(m);
      var value = TranslateExpression(u.Item3);
      updates.Add(new DatatypeUpdatePair(id, value));
    }
    return new DatatypeUpdateExpr(datatypeValue, updates);
  }

  private BinaryExpr TranslateBinaryExpr(Dafny.BinaryExpr be) {
    return new BinaryExpr(TranslateBinaryOpcode(be.Op),
      TranslateExpression(be.E0), TranslateExpression(be.E1));
  }

  private BinaryExpr.Opcode TranslateBinaryOpcode(Dafny.BinaryExpr.Opcode op) {
    return op switch {
      Dafny.BinaryExpr.Opcode.Iff => BinaryExpr.Opcode.Iff,
      Dafny.BinaryExpr.Opcode.Imp => BinaryExpr.Opcode.Imp,
      Dafny.BinaryExpr.Opcode.Exp => BinaryExpr.Opcode.Exp,
      Dafny.BinaryExpr.Opcode.And => BinaryExpr.Opcode.And,
      Dafny.BinaryExpr.Opcode.Or => BinaryExpr.Opcode.Or,
      Dafny.BinaryExpr.Opcode.Eq => BinaryExpr.Opcode.Eq,
      Dafny.BinaryExpr.Opcode.Neq => BinaryExpr.Opcode.Neq,
      Dafny.BinaryExpr.Opcode.Lt => BinaryExpr.Opcode.Lt,
      Dafny.BinaryExpr.Opcode.Le => BinaryExpr.Opcode.Le,
      Dafny.BinaryExpr.Opcode.Ge => BinaryExpr.Opcode.Ge,
      Dafny.BinaryExpr.Opcode.Gt => BinaryExpr.Opcode.Gt,
      Dafny.BinaryExpr.Opcode.Disjoint => BinaryExpr.Opcode.Disjoint,
      Dafny.BinaryExpr.Opcode.In => BinaryExpr.Opcode.In,
      Dafny.BinaryExpr.Opcode.NotIn => BinaryExpr.Opcode.NotIn,
      Dafny.BinaryExpr.Opcode.LeftShift => BinaryExpr.Opcode.LeftShift,
      Dafny.BinaryExpr.Opcode.RightShift => BinaryExpr.Opcode.RightShift,
      Dafny.BinaryExpr.Opcode.Add => BinaryExpr.Opcode.Add,
      Dafny.BinaryExpr.Opcode.Sub => BinaryExpr.Opcode.Sub,
      Dafny.BinaryExpr.Opcode.Mul => BinaryExpr.Opcode.Mul,
      Dafny.BinaryExpr.Opcode.Div => BinaryExpr.Opcode.Div,
      Dafny.BinaryExpr.Opcode.Mod => BinaryExpr.Opcode.Mod,
      Dafny.BinaryExpr.Opcode.BitwiseAnd => BinaryExpr.Opcode.BitwiseAnd,
      Dafny.BinaryExpr.Opcode.BitwiseOr => BinaryExpr.Opcode.BitwiseOr,
      Dafny.BinaryExpr.Opcode.BitwiseXor => BinaryExpr.Opcode.BitwiseXor,
      _ => throw new UnsupportedTranslationException(op),
    };
  }

  private UnaryExpr TranslateUnaryExpr(Dafny.UnaryOpExpr ue) {
    return new UnaryExpr(
      TranslateUnaryOpcode(ue.Op), TranslateExpression(ue.E));
  }

  private UnaryExpr.Opcode TranslateUnaryOpcode(Dafny.UnaryOpExpr.Opcode op) {
    return op switch {
      Dafny.UnaryOpExpr.Opcode.Not => UnaryExpr.Opcode.Not,
      Dafny.UnaryOpExpr.Opcode.Cardinality => UnaryExpr.Opcode.Cardinality,
      Dafny.UnaryOpExpr.Opcode.Fresh => UnaryExpr.Opcode.Fresh,
      Dafny.UnaryOpExpr.Opcode.Allocated => UnaryExpr.Opcode.Allocated,
      _ => throw new UnsupportedTranslationException(op),
    };
  }

}
