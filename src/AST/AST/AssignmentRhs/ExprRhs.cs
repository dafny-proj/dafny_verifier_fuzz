namespace AST;

public class ExprRhs
: AssignmentRhs, ConstructableFromDafny<Dafny.ExprRhs, ExprRhs> {
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

  public override IEnumerable<Node> Children => new[] { Expr };
  public override AssignmentRhs Clone() {
    return new ExprRhs(Expr.Clone());
  }
  public override void ReplaceChild(Node oldChild, Node newChild) {
    if (oldChild is not Expression || newChild is not Expression) {
      throw new ArgumentException($"Children of `{this.GetType()}` should be of expression type.");
    }
    if (oldChild != Expr) {
      throw new ArgumentException("Cannot find child in ExprRhs.");
    }
    Expr = (newChild as Expression)!;
  }
}
