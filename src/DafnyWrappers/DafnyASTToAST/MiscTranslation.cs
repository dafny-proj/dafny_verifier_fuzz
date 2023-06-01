using Dafny = Microsoft.Dafny;

namespace AST.Translation;

public partial class ASTTranslator {
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

  private QuantifierDomain TranslateQuantifierDomain(
  IEnumerable<Dafny.BoundVar> vars, Dafny.Expression? range) {
    var vs = vars.Select(TranslateBoundVar);
    // Quantified vars may be explicitly typed, but this information is lost 
    // during desugaring of the quantifier domain. Assume explicit to be safe.
    foreach (var v in vs) {
      v.ExplicitType = v.Type;
    }
    return new QuantifierDomain(vars: vs,
      range: range == null ? null : TranslateExpression(range));
  }

  private Matcher TranslateMatcher(Dafny.ExtendedPattern p) {
    if (p is Dafny.LitPattern lp) {
      return new ExpressionMatcher(TranslateExpression(lp.OrigLit));
    } else if (p is Dafny.DisjunctivePattern dp) {
      return new DisjunctiveMatcher(dp.Alternatives.Select(TranslateMatcher));
    } else if (p is Dafny.IdPattern ip) {
      if (ip.BoundVar != null) {
        return new BindingMatcher(TranslateVariable(ip.BoundVar));
      } else if (ip.Arguments != null && ip.Ctor != null) {
        return new DestructuringMatcher(
          (DatatypeConstructorDecl)TranslateDeclRef(ip.Ctor),
          ip.Arguments.Select(TranslateMatcher));
      }
    }
    throw new UnsupportedTranslationException(p);
  }

}
