using Dafny = Microsoft.Dafny;

namespace AST_new.Translation;

public partial class DafnyASTTranslator {
  private BoundVar TranslateBoundVar(Dafny.BoundVar bv) {
    return new BoundVar(bv.Name, TranslateType(bv.Type));
  }

  private Formal TranslateFormal(Dafny.Formal f) {
    return new Formal(f.Name, TranslateType(f.Type),
      f.DefaultValue == null ? null : TranslateExpression(f.DefaultValue));
  }

}
