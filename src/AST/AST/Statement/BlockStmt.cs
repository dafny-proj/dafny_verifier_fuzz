namespace AST;

public partial class BlockStmt : Statement {
  public readonly List<Statement> Body = new();

  public BlockStmt() { }

  public BlockStmt(IEnumerable<Statement> body) {
    Body.AddRange(body);
  }

  public void Append(Statement s) => Body.Add(s);
  public void Prepend(Statement s) => Body.Insert(0, s);
  public void Replace(Statement s, Statement newS) => Replace(s, new[] { newS });
  public void Replace(Statement s, IEnumerable<Statement> newS) {
    var index = Body.FindIndex(c => c == s);
    if (index == -1) {
      throw new ChildNotFoundException(this, s);
    }
    Body.RemoveAt(index);
    Body.InsertRange(index, newS);
  }
  public void Remove(Statement s) => Body.Remove(s);

  public override IEnumerable<Node> Children => Body;
}