namespace AST;

public class ExprRhs
: AssignmentRhs, ConstructableFromDafny<Dafny.ExprRhs, ExprRhs> {
  public override IEnumerable<Node> Children => new[] { Expr };
  public Expression Expr;

  public ExprRhs(Expression expr) {
    Expr = expr;
  }

  private ExprRhs(Dafny.ExprRhs exprRhsDafny) {
    Expr = Expression.FromDafny(exprRhsDafny.Expr);
  }
  public static ExprRhs FromDafny(Dafny.ExprRhs dafnyNode) {
    return new ExprRhs(dafnyNode);
  }

  public override AssignmentRhs Clone() {
    return new ExprRhs(Expr.Clone());
  }
}
