using Dafny = Microsoft.Dafny;

namespace AST_new.Translation;

public partial class DafnyASTTranslator {
  private Variable CreateVariable(Dafny.IVariable dv) {
    var name = dv.Name;
    var type = TranslateType(dv.Type);
    return dv switch {
      Dafny.LocalVariable lv => new LocalVar(name, type, explicitType:
        lv.IsTypeExplicit ? TranslateType(lv.OptionalType) : null),
      Dafny.BoundVar bv => new BoundVar(name, type, explicitType:
        bv.IsTypeExplicit ? type : null),
      Dafny.Formal f => new Formal(name, type, defaultValue:
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
  Dafny.Specification<T>? es) where T : Dafny.Node {
    return TranslateSpecification<T>(st, es?.Expressions);
  }

  private Specification? TranslateSpecification<T>(Specification.Type st,
  List<T>? es) where T : Dafny.Node {
    if (es?.Count > 0) {
      return new Specification(st, es.Select(TranslateExpression));
    }
    return null;
  }

  private ExpressionPair TranslateExpressionPair(Dafny.ExpressionPair ep) {
    return new ExpressionPair(
      TranslateExpression(ep.A), TranslateExpression(ep.B));
  }

}