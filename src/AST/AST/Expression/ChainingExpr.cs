namespace AST;

public class ChainingExpression
: Expression, ConstructableFromDafny<Dafny.ChainingExpression, ChainingExpression> {
  // TODO: Is this correct? Or should we list the chaining expression desugared as binary expressions?
  public override IEnumerable<Node> Children => Operands;
  public List<Expression> Operands = new List<Expression>();
  public List<BinaryExpr.Opcode> Operators = new List<BinaryExpr.Opcode>();
  protected Type _Type;
  public override Type Type { get => _Type; }

  private ChainingExpression(Dafny.ChainingExpression ced) {
    Operands.AddRange(ced.Operands.Select(Expression.FromDafny));
    Operators.AddRange(ced.Operators.Select(BinaryExpr.FromDafny));
    _Type = Type.FromDafny(ced.Type);
  }

  public static ChainingExpression FromDafny(Dafny.ChainingExpression dafnyNode) {
    return new ChainingExpression(dafnyNode);
  }
}