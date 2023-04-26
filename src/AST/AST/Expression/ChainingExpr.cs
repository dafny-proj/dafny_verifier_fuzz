namespace AST;

public class ChainingExpression
: Expression, ConstructableFromDafny<Dafny.ChainingExpression, ChainingExpression> {
  public List<Expression> Operands = new List<Expression>();
  public List<BinaryExpr.Opcode> Operators = new List<BinaryExpr.Opcode>();

  private ChainingExpression(Dafny.ChainingExpression ced) {
    Operands.AddRange(ced.Operands.Select(Expression.FromDafny));
    Operators.AddRange(ced.Operators.Select(BinaryExpr.FromDafny));
  }

  public static ChainingExpression FromDafny(Dafny.ChainingExpression dafnyNode) {
    return new ChainingExpression(dafnyNode);
  }
}