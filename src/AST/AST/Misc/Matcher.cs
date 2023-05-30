namespace AST;

public abstract partial class Matcher : Node { }
public partial class WildcardMatcher : Matcher { }
public partial class ExpressionMatcher : Matcher { }
public partial class BindingMatcher : Matcher { }
public partial class DestructuringMatcher : Matcher { }
public partial class DisjunctiveMatcher : Matcher { }

public partial class WildcardMatcher : Matcher {
  public override IEnumerable<Node> Children => Enumerable.Empty<Node>();
}

public partial class ExpressionMatcher : Matcher {
  public Expression E { get; set; }

  public ExpressionMatcher(Expression e) { E = e; }

  public override IEnumerable<Node> Children => new[] { E };
}

public partial class BindingMatcher : Matcher {
  public Variable Var { get; }

  public BindingMatcher(Variable var) { Var = var; }

  public override IEnumerable<Node> Children => new[] { Var };
}

public partial class DestructuringMatcher : Matcher {
  public DatatypeConstructorDecl Constructor { get; }
  public readonly List<Matcher> ArgumentMatchers = new();

  public DestructuringMatcher(DatatypeConstructorDecl constructor,
  IEnumerable<Matcher> argumentMatchers) {
    Constructor = constructor;
    ArgumentMatchers.AddRange(argumentMatchers);
  }

  public override IEnumerable<Node> Children => ArgumentMatchers;
}

public partial class DisjunctiveMatcher : Matcher {
  public readonly List<Matcher> Matchers = new();

  public DisjunctiveMatcher(IEnumerable<Matcher> matchers) {
    Matchers.AddRange(matchers);
  }

  public override IEnumerable<Node> Children => Matchers;
}
