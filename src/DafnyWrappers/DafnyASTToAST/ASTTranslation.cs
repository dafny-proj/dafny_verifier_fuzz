using Dafny = Microsoft.Dafny;

namespace AST_new.Translation;

public class UnsupportedTranslationException : Exception {
  public UnsupportedTranslationException(string message) : base(message) { }

  public UnsupportedTranslationException(Dafny.Node trigger)
  : this($"Unsupported AST translation from `{trigger.GetType()}`.") { }
}

// Requires Dafny program to be resolved.
public partial class DafnyASTTranslator {
  private Dictionary<Dafny.Declaration, Declaration> TranslatedDecls = new();
  private void AddTranslatedDecl(Dafny.Declaration dd, Declaration d) {
    if (TranslatedDecls.ContainsKey(dd)) {
      throw new ArgumentException(
        $"Translated declaration already exists for `{dd}`.");
    }
    TranslatedDecls[dd] = d;
  }
  private Declaration GetTranslatedDecl(Dafny.Declaration dd) {
    if (!TranslatedDecls.ContainsKey(dd)) {
      throw new ArgumentException(
        $"Could not find translated declaration for `{dd}`.");
    }
    return TranslatedDecls[dd];
  }
  private Declaration GetTranslatedDeclOrCreateSkeleton(Dafny.Declaration dd) {
    if (!TranslatedDecls.ContainsKey(dd)) {
      AddTranslatedDecl(dd, CreateDeclSkeleton(dd));
    }
    return TranslatedDecls[dd];
  }

  public static Program TranslateDafnyProgram(Dafny.Program p) {
    return new DafnyASTTranslator().TranslateProgram(p);
  }

  private Program TranslateProgram(Dafny.Program p) {
    return new Program(TranslateModule(p.DefaultModule));
  }

}
