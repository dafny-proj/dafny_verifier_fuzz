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

}
