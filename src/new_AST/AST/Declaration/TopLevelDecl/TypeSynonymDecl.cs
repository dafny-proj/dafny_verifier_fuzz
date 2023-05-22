namespace AST_new;

public partial class TypeSynonymDecl : TopLevelDecl {
  public override string Name { get; protected set; }
  public readonly List<TypeParameterDecl> TypeParams = new();
  public Type BaseType { get; }

  public TypeSynonymDecl(string name, Type baseType,
  IEnumerable<TypeParameterDecl>? typeParams = null) {
    Name = name;
    BaseType = baseType;
    if (typeParams != null) {
      TypeParams.AddRange(typeParams);
    }
  }

  public override IEnumerable<Node> Children
    => TypeParams.Append<Node>(BaseType);
}
