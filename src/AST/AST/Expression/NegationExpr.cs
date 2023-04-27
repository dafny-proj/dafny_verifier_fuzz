namespace AST;

public class NegationExpression
: Expression, ConstructableFromDafny<Dafny.NegationExpression, NegationExpression> {
  public override IEnumerable<Node> Children => new Node[] { E };

  public Expression E { get; set; }
  private Type _Type;
  public override Type Type { get => _Type; }

  private NegationExpression(Dafny.NegationExpression negExprDafny) {
    E = Expression.FromDafny(negExprDafny.E);
    _Type = Type.FromDafny(negExprDafny.Type);
  }

  public static NegationExpression FromDafny(Dafny.NegationExpression dafnyNode) {
    return new NegationExpression(dafnyNode);
  }
}
