using Dafny = Microsoft.Dafny;

namespace AST.Translation;

public class UnsupportedTranslationException : Exception {
  public UnsupportedTranslationException(string message) : base(message) { }

  public UnsupportedTranslationException(object trigger)
  : this($"Unsupported AST translation from `{trigger.GetType()}`.") { }
}

// Requires Dafny program to be resolved.
public partial class ASTTranslator {
  private Dictionary<Dafny.Declaration, Declaration> SkeletonDecls = new();
  private Dictionary<Dafny.Declaration, Declaration> TranslatedDecls = new();

  private bool HasTranslatedDecl(Dafny.Declaration dd)
    => TranslatedDecls.ContainsKey(dd);
  private Declaration GetTranslatedDecl(Dafny.Declaration dd)
    => TranslatedDecls[dd];
  private bool HasSkeletonDecl(Dafny.Declaration dd)
    => SkeletonDecls.ContainsKey(dd);
  private Declaration GetSkeletonDecl(Dafny.Declaration dd)
    => TranslatedDecls[dd];
  private void MarkDeclSkeleton(Dafny.Declaration dd, Declaration d) {
    SkeletonDecls.Add(dd, d);
  }
  private void MarkDeclTranslated(Dafny.Declaration dd, Declaration d) {
    SkeletonDecls.Remove(dd);
    TranslatedDecls.Add(dd, d);
  }

  private Dictionary<Dafny.IVariable, Variable> TranslatedVariables = new();
  private void AddTranslatedVar(Dafny.IVariable dv, Variable v) {
    if (TranslatedVariables.ContainsKey(dv)) {
      throw new ArgumentException(
        $"Translated variable already exists for `{dv}`.");
    }
    TranslatedVariables[dv] = v;
  }
  private Variable GetTranslatedVar(Dafny.IVariable dv) {
    if (!TranslatedVariables.ContainsKey(dv)) {
      throw new ArgumentException(
        $"Could not find translated variable for `{dv}`.");
    }
    return TranslatedVariables[dv];
  }
  private Variable GetOrCreateTranslatedVar(Dafny.IVariable dv) {
    if (!TranslatedVariables.ContainsKey(dv)) {
      AddTranslatedVar(dv, CreateVariable(dv));
    }
    return TranslatedVariables[dv];
  }

  public static Program TranslateDafnyProgram(Dafny.Program p) {
    return new ASTTranslator().TranslateProgram(p);
  }

  private Program TranslateProgram(Dafny.Program p) {
    return new Program(TranslateModuleDecl(p.DefaultModule));
  }
}
