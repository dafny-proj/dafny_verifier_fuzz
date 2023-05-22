namespace AST_new;

public partial class TypeParameterDecl : TopLevelDecl {
  public override string Name { get; protected set; }
  public TypeParameterDecl(string name) {
    Name = name;
  }

  public override IEnumerable<Node> Children => Enumerable.Empty<Node>();
}
