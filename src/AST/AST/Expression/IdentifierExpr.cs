namespace AST;

// Subclasses: AutoGhostIdentExpr, ResolverIdentExpr
public class IdentifierExpr
: Expression, ConstructableFromDafny<Dafny.IdentifierExpr, IdentifierExpr> {
  public string Name { get; set; }
  protected Type _Type;
  public override Type Type { get => _Type; }

  private IdentifierExpr(Dafny.IdentifierExpr identExprDafny) {
    Name = identExprDafny.Name;
    _Type = Type.FromDafny(identExprDafny.Type);
  }

  public static IdentifierExpr FromDafny(Dafny.IdentifierExpr dafnyNode) {
    return new IdentifierExpr(dafnyNode);
  }
}