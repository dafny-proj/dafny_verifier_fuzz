namespace AST_new;

public partial class BlockStmt : Statement {
  public readonly List<Statement> Body = new();

  public BlockStmt() { }

  public BlockStmt(IEnumerable<Statement> body) {
    Body.AddRange(body);
  }

  public override IEnumerable<Node> Children => Body;
}