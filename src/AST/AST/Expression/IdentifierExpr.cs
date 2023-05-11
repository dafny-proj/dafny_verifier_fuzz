namespace AST;

// Subclasses: AutoGhostIdentExpr, ResolverIdentExpr
public class IdentifierExpr
: Expression, ConstructableFromDafny<Dafny.IdentifierExpr, IdentifierExpr> {
  public string Name { get; }
  protected Type _Type;
  public override Type Type { get => _Type; }

  public IdentifierExpr(string name, Type type) {
    Name = name;
    _Type = type;
  }

  private IdentifierExpr(Dafny.IdentifierExpr identExprDafny) {
    Name = identExprDafny.Name;
    _Type = Type.FromDafny(identExprDafny.Type);
  }

  public static IdentifierExpr FromDafny(Dafny.IdentifierExpr dafnyNode) {
    return new IdentifierExpr(dafnyNode);
  }

  public override Expression Clone() {
    // TODO: clone type?
    return new IdentifierExpr(Name, Type);
  }
}