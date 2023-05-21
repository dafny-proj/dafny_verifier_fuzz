namespace AST_new;

public partial class ReturnStmt : Statement {
  public UpdateStmt? Returns { get; }

  public bool HasReturns() => Returns != null;

  public ReturnStmt(UpdateStmt returns) {
    Returns = returns;
  }
}