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

  public override void ReplaceChild(Node oldChild, Node newChild) {
    if (oldChild is not Statement || newChild is not Statement) {
      throw new ArgumentException("Children of block statement should be of statement type.");
    }
    var i = Body.FindIndex(c => c == oldChild);
    if (i == -1) {
      throw new Exception("Cannot find child in block statement.");
    }
    Body[i] = (Statement)newChild;
  }
}