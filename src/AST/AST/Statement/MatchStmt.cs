namespace AST;

public partial class MatchStmt : Statement {
  public Expression Selector { get; set; }
  public readonly List<MatchStmtCase> Cases = new();

  public MatchStmt(Expression selector, IEnumerable<MatchStmtCase> cases) {
    Selector = selector;
    Cases.AddRange(cases);
  }

  public override IEnumerable<Node> Children => Cases.Prepend<Node>(Selector);
}
