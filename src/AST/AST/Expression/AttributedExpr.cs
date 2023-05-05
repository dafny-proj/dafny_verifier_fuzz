namespace AST;

public class AttributedExpression
: Node, ConstructableFromDafny<Dafny.AttributedExpression, AttributedExpression> {
  // TODO: Attributes, Label
  public override IEnumerable<Node> Children => new Node[] { E };
  public Expression E;

  public AttributedExpression(Expression e) {
    E = e;
  }

  private AttributedExpression(Dafny.AttributedExpression attributedExprDafny)
  : this(Expression.FromDafny(attributedExprDafny.E)) { }

  public static AttributedExpression FromDafny(Dafny.AttributedExpression dafnyNode) {
    return new AttributedExpression(dafnyNode);
  }

  public override AttributedExpression Clone() {
    return new AttributedExpression(E.Clone());
  }
}
