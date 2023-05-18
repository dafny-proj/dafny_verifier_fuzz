using Dafny = Microsoft.Dafny;

namespace AST_new.Translation;

public partial class DafnyASTTranslator {
  public Statement TranslateStatement(Dafny.Statement s) {
    return s switch {
      Dafny.BlockStmt bs => TranslateBlockStmt(bs),
      Dafny.PrintStmt ps => TranslatePrintStmt(ps),
      _ => throw new UnsupportedTranslationException(s),
    };
  }

  public BlockStmt TranslateBlockStmt(Dafny.BlockStmt bs) {
    return new BlockStmt(bs.Body.Select(TranslateStatement));
  }

  public PrintStmt TranslatePrintStmt(Dafny.PrintStmt ps) {
    return new PrintStmt(ps.Args.Select(TranslateExpression));
  }
}
