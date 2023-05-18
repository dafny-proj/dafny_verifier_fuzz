using Dafny = Microsoft.Dafny;

namespace AST_new.Translation;

public partial class DafnyASTTranslator {
  private Formal TranslateFormal(Dafny.Formal f) {
    return new Formal(f.Name, TranslateType(f.Type),
      f.DefaultValue == null ? null : TranslateExpression(f.DefaultValue));
  }

}
