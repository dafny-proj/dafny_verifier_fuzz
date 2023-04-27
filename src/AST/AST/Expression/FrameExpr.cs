namespace AST;

public class FrameExpression
: Node, ConstructableFromDafny<Dafny.FrameExpression, FrameExpression> {
  public override IEnumerable<Node> Children => new Node[] { E };
  
  // TODO: FieldName
  public Expression E; // pre-resolution
  private FrameExpression(Dafny.FrameExpression frameExprDafny) {
    E = Expression.FromDafny(frameExprDafny.E);
  }
  public static FrameExpression FromDafny(Dafny.FrameExpression dafnyNode) {
    return new FrameExpression(dafnyNode);
  }
}
