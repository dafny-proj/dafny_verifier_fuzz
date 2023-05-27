namespace AST;

public partial class FieldDecl : MemberDecl {
  public override string Name { get; protected set; }
  public virtual Type Type { get; protected set; }

  public FieldDecl(TopLevelDecl enclosingDecl, string name, Type type)
  : base(enclosingDecl) {
    Name = name;
    Type = type;
  }

  public override IEnumerable<Node> Children => new[] { Type };
}
