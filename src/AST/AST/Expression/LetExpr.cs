namespace AST;

public partial class LetExpr : Expression {
  public readonly List<VarExpressionPair> Vars = new();
  public Expression Body { get; set; }

  public LetExpr(IEnumerable<VarExpressionPair> vars, Expression body) {
    Vars.AddRange(vars);
    Body = body;
  }

  public override IEnumerable<Node> Children => Vars.Append<Node>(Body);
}