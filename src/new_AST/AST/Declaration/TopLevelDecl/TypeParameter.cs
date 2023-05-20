namespace AST_new;

public partial class TypeParameter : TopLevelDecl {
  public override string Name { get; protected set; }
  public TypeParameter(string name) {
    Name = name;
  }
}
