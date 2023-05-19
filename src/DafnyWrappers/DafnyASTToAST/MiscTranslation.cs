using Dafny = Microsoft.Dafny;

namespace AST_new.Translation;

public partial class DafnyASTTranslator {
  private Variable CreateVariable(Dafny.IVariable dv) {
    return dv switch {
      Dafny.LocalVariable lv => new LocalVar(lv.Name, TranslateType(lv.Type)),
      Dafny.BoundVar bv => new BoundVar(bv.Name, TranslateType(bv.Type)),
      Dafny.Formal f => new Formal(
        f.Name, TranslateType(f.Type),
        f.DefaultValue == null ? null : TranslateExpression(f.DefaultValue)),
      _ => throw new UnsupportedTranslationException(dv),
    };
  }

  private Variable TranslateVariable(Dafny.IVariable v) {
    return GetOrCreateTranslatedVar(v);
  }

  private LocalVar TranslateLocalVar(Dafny.LocalVariable lv) {
    return (LocalVar)GetOrCreateTranslatedVar(lv);
  }

  private BoundVar TranslateBoundVar(Dafny.BoundVar bv) {
    return (BoundVar)GetOrCreateTranslatedVar(bv);
  }

  private Formal TranslateFormal(Dafny.Formal f) {
    return (Formal)GetOrCreateTranslatedVar(f);
  }

  private Specification? TranslateSpecification<T>(Specification.Type st,
  Dafny.Specification<T> es) where T : Dafny.Node {
    return TranslateSpecification<T>(st, es.Expressions);
  }

  private Specification? TranslateSpecification<T>(Specification.Type st,
  List<T> es) where T : Dafny.Node {
    if (es.Count > 0) {
      return new Specification(st, es.Select(TranslateExpression));
    }
    return null;
  }

}
