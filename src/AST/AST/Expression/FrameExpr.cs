namespace AST;

public class FrameExpression
: Node, ConstructableFromDafny<Dafny.FrameExpression, FrameExpression> {
  public override IEnumerable<Node> Children => new Node[] { E };

  // TODO: FieldName
  public Expression E;

  public FrameExpression(Expression e) {
    E = e;
  }

  private FrameExpression(Dafny.FrameExpression frameExprDafny)
  : this(Expression.FromDafny(frameExprDafny.E)) { }

  public static FrameExpression FromDafny(Dafny.FrameExpression dafnyNode) {
    return new FrameExpression(dafnyNode);
  }

  public override FrameExpression Clone() {
    return new FrameExpression(E.Clone());
  }
}
