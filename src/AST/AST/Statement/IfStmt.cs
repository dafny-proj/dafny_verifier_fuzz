namespace AST;

public class IfStmt
: Statement, ConstructableFromDafny<Dafny.IfStmt, IfStmt> {
  public override IEnumerable<Node> Children {
    get {
      if (Guard != null) {
        yield return Guard;
      }
      yield return Thn;
      if (Els != null) {
        yield return Els;
      }
    }
  }

  public Expression? Guard { get; set; }
  public BlockStmt Thn { get; set; }
  public Statement? Els { get; set; }

  public IfStmt(Expression? guard, BlockStmt thn, Statement? els = null) {
    Guard = guard;
    Thn = thn;
    Els = els;
  }

  private IfStmt(Dafny.IfStmt ifStmtDafny) {
    Guard = ifStmtDafny.Guard == null ? null : Expression.FromDafny(ifStmtDafny.Guard);
    Thn = BlockStmt.FromDafny(ifStmtDafny.Thn);
    Els = ifStmtDafny.Els == null ? null : Statement.FromDafny(ifStmtDafny.Els);
  }

  public static IfStmt FromDafny(Dafny.IfStmt dafnyNode) {
    return new IfStmt(dafnyNode);
  }

  public override Statement Clone() {
    return new IfStmt(Guard?.Clone(), Thn.Clone(), Els?.Clone());
  }
}