using Dafny = Microsoft.Dafny;

namespace AST_new.Translation;

public partial class DafnyASTTranslator {
  public Type TranslateType(Dafny.Type t) {
    throw new UnsupportedTranslationException(t);
  }

}
