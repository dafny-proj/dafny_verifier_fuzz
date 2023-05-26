namespace AST_new;

public partial class ReturnStmt : Statement {
  public UpdateStmt? Returns { get; set; }

  public bool HasReturns() => Returns != null;

  public ReturnStmt(UpdateStmt? returns = null) {
    Returns = returns;
  }

  public override IEnumerable<Node> Children
    => Returns?.Rhss ?? Enumerable.Empty<Node>();
}