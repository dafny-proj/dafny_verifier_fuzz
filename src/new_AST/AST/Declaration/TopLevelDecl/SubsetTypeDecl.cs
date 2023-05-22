namespace AST_new;

public partial class SubsetTypeDecl : TopLevelDecl {
  public override string Name { get; protected set; }
  public readonly List<TypeParameterDecl> TypeParams = new();
  public BoundVar BaseIdent;
  public Type BaseType => BaseIdent.Type;
  public Expression Constraint { get; }
  // TODO: Witness.

  public SubsetTypeDecl(string name, BoundVar baseIdent, Expression constraint,
  IEnumerable<TypeParameterDecl>? typeParams = null) {
    Name = name;
    BaseIdent = baseIdent;
    Constraint = constraint;
    if (typeParams != null) {
      TypeParams.AddRange(typeParams);
    }
  }

  public override IEnumerable<Node> Children
    => TypeParams.Append<Node>(BaseIdent).Append<Node>(Constraint);
}
