namespace AST;

public class NegationExpression
: Expression, ConstructableFromDafny<Dafny.NegationExpression, NegationExpression> {
  public Expression E { get; set; }

  private NegationExpression(Dafny.NegationExpression negExprDafny) {
    E = Expression.FromDafny(negExprDafny.E);
  }

  public static NegationExpression FromDafny(Dafny.NegationExpression dafnyNode) {
    return new NegationExpression(dafnyNode);
  }
}
