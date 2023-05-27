namespace AST;

public partial class IfStmt : Statement {
  public Expression? Guard { get; set; }
  public BlockStmt Thn { get; set; }
  public Statement? Els { get; set; }

  public bool HasElse() => Els != null;

  public IfStmt(Expression? guard, BlockStmt thn, Statement? els = null) {
    Guard = guard;
    Thn = thn;
    Els = els;
  }

  public override IEnumerable<Node> Children {
    get {
      if (Guard != null) { yield return Guard; }
      yield return Thn;
      if (Els != null) { yield return Els; }
    }
  }
}
