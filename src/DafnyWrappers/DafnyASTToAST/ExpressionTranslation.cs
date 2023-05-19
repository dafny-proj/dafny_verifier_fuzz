using System.Numerics;
using Dafny = Microsoft.Dafny;

namespace AST_new.Translation;

public partial class DafnyASTTranslator {
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
      Dafny.ConcreteSyntaxExpression cse => cse switch {
        Dafny.ParensExpression pe => TranslateParensExpr(pe),
        _ => TranslateExpression(cse.ResolvedExpression),
      },
      _ => throw new UnsupportedTranslationException(e),
    };
  }

  private LiteralExpr TranslateLiteralExpr(Dafny.LiteralExpr le) {
    if (le.Value is bool b) {
      return new BoolLiteralExpr(b);
    } else if (le.Value is BigInteger i) {
      return new IntLiteralExpr(i);
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

  private MemberSelectExpr TranslateMemberSelectExpr(Dafny.MemberSelectExpr mse) {
    return new MemberSelectExpr(
      TranslateExpression(mse.Obj), TranslateMemberDecl(mse.Member));
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

  private ExpressionPair TranslateExpressionPair(Dafny.ExpressionPair ep) {
    return new ExpressionPair(
      TranslateExpression(ep.A), TranslateExpression(ep.B));
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
