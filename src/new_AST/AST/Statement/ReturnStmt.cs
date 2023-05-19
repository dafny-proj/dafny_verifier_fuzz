namespace AST_new;

public partial class ReturnStmt : Statement {
  public UpdateStmt Update { get; }

  public ReturnStmt(UpdateStmt update) {
    Update = update;
  }
}