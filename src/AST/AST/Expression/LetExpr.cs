namespace AST;

public partial class LetExpr : Expression {
  public readonly List<KeyValuePair<BoundVar, Expression>> Vars = new();
  public Expression Body { get; set; }

  public LetExpr(IEnumerable<KeyValuePair<BoundVar, Expression>> vars,
  Expression body) {
    Vars.AddRange(vars);
    Body = body;
  }

  public override IEnumerable<Node> Children => Vars.Append<Node>(Body);
}