namespace AST;

public class IfStmt
: Statement, ConstructableFromDafny<Dafny.IfStmt, IfStmt> {

  public Expression? Guard { get; set; }
  public BlockStmt Thn { get; set; }
  public Statement? Els { get; set; }

  private IfStmt(Dafny.IfStmt ifStmtDafny) {
    Guard = ifStmtDafny.Guard == null ? null : Expression.FromDafny(ifStmtDafny.Guard);
    Thn = BlockStmt.FromDafny(ifStmtDafny.Thn);
    Els = ifStmtDafny.Els == null ? null : Statement.FromDafny(ifStmtDafny.Els);
  }

  public static IfStmt FromDafny(Dafny.IfStmt dafnyNode) {
    return new IfStmt(dafnyNode);
  }
}