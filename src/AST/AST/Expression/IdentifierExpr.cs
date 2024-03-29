namespace AST;

public partial class IdentifierExpr : Expression {
  public Variable Var { get; set; }
  public string Name => Var.Name;
  public override Type Type => Var.Type;

  public IdentifierExpr(Variable var) {
    Var = var;
  }

  public override IEnumerable<Node> Children => Enumerable.Empty<Node>();
}
