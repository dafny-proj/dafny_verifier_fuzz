namespace AST;

// Subclasses: AutoGhostIdentExpr, ResolverIdentExpr
public class IdentifierExpr
: Expression, ConstructableFromDafny<Dafny.IdentifierExpr, IdentifierExpr> {
  public string Name { get; set; }

  private IdentifierExpr(Dafny.IdentifierExpr identExprDafny) {
    Name = identExprDafny.Name;
  }

  public static IdentifierExpr FromDafny(Dafny.IdentifierExpr dafnyNode) {
    return new IdentifierExpr(dafnyNode);
  }
}