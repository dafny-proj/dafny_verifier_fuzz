namespace AST_new;

public partial class IdentifierExpr : Expression {
  public Variable Var { get; }
  public string Name => Var.Name;
  public override Type Type => Var.Type;

  public IdentifierExpr(Variable var) {
    Var = var;
  }
}
