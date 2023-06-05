namespace AST;

public partial class FieldDecl : MemberDecl {
  public override string Name { get; protected set; }
  public virtual Type Type { get; protected set; }
  public virtual bool IsBuiltIn { get; }

  public FieldDecl(TopLevelDecl enclosingDecl, string name, Type type, bool isBuiltIn = false)
  : base(enclosingDecl) {
    Name = name;
    Type = type;
    IsBuiltIn = isBuiltIn;
  }

  public override IEnumerable<Node> Children => new[] { Type };
}
