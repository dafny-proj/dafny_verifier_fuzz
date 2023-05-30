namespace AST;

public partial class MatchExpr : Expression { }

public partial class MatchExpr : Expression {
  public Expression Selector { get; set; }
  public readonly List<MatchExprCase> Cases = new();

  public MatchExpr(Expression selector, IEnumerable<MatchExprCase> cases) {
    Selector = selector;
    Cases.AddRange(cases);
  }

  public override IEnumerable<Node> Children => Cases.Prepend<Node>(Selector);
}
