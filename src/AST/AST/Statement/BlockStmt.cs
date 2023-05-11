namespace AST;

public class BlockStmt
: Statement, ConstructableFromDafny<Dafny.BlockStmt, BlockStmt> {
  public override IEnumerable<Node> Children => Body;

  public List<Statement> Body = new List<Statement>();
  private BlockStmt(Dafny.BlockStmt blockStmtDafny) {
    Body.AddRange(blockStmtDafny.Body.Select(Statement.FromDafny));
  }

  public BlockStmt(IEnumerable<Statement>? body = null) {
    if (body != null) {
      Body.AddRange(body);
    }
  }

  // TODO: Prepend is not super efficient
  public void Prepend(Statement s) => Body.Insert(0, s);
  public void Append(Statement s) => Body.Add(s);
  public void Append(List<Statement> ss) => Body.AddRange(ss);

  public static BlockStmt FromDafny(Dafny.BlockStmt dafnyNode) {
    return new BlockStmt(dafnyNode);
  }

  // TODO: Make this a method of the Node class?
  public void RemoveChild(Statement s) {
    var removed = Body.Remove(s);
    if (!removed) {
      throw new Exception("Failed to remove child in block statement.");
    }
  }
  
  public override void ReplaceChild(Node oldChild, Node newChild) {
    if (oldChild is not Statement || newChild is not Statement) {
      throw new ArgumentException("Children of block statement should be of statement type.");
    }
    var i = Body.FindIndex(c => c == oldChild);
    if (i == -1) {
      throw new Exception("Cannot find child in block statement.");
    }
    if (newChild is BlockStmt b) {
      // To allow replacement of a node within a block with multiple nodes.
      Body.RemoveAt(i);
      Body.InsertRange(i, b.Body);
    } else {
      Body[i] = (Statement)newChild;
    }
  }

  public override BlockStmt Clone() {
    return new BlockStmt(Body.Select(s => s.Clone()));
  }
}