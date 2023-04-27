namespace AST;

public class BlockStmt
: Statement, ConstructableFromDafny<Dafny.BlockStmt, BlockStmt> {
  public override IEnumerable<Node> Children => Body;

  public List<Statement> Body = new List<Statement>();
  private BlockStmt(Dafny.BlockStmt blockStmtDafny) {
    Body.AddRange(blockStmtDafny.Body.Select(Statement.FromDafny));
  }
  public static BlockStmt FromDafny(Dafny.BlockStmt dafnyNode) {
    return new BlockStmt(dafnyNode);
  }
}