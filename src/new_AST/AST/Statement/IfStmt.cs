namespace AST_new;

public partial class IfStmt : Statement {
  public Expression? Guard { get; }
  public BlockStmt Thn { get; }
  public Statement? Els { get; }

  public bool HasElse() => Els != null;

  public IfStmt(Expression? guard, BlockStmt thn, Statement? els = null) {
    Guard = guard;
    Thn = thn;
    Els = els;
  }
}
