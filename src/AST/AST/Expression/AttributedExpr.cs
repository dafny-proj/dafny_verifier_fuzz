namespace AST;

public class AttributedExpression
: Node, ConstructableFromDafny<Dafny.AttributedExpression, AttributedExpression> {
  // TODO: Attributes, Label
  public Expression E;
  private AttributedExpression(Dafny.AttributedExpression attributedExprDafny) {
    E = Expression.FromDafny(attributedExprDafny.E);
  }
  public static AttributedExpression FromDafny(Dafny.AttributedExpression dafnyNode) {
    return new AttributedExpression(dafnyNode);
  }
}
