namespace AST;

public partial class ITEExpr : Expression {
  public Expression Guard { get; set; }
  public Expression Thn { get; set; }
  public Expression Els { get; set; }

  public ITEExpr(Expression guard, Expression thn, Expression els) {
    Guard = guard;
    Thn = thn;
    Els = els;
  }

  public override IEnumerable<Node> Children => new[] { Guard, Thn, Els };
}
