namespace AST;

public static class NodeFactory {
  public static IntLiteralExpr CreateIntLiteral(int i)
    => new IntLiteralExpr(i);

  private static BinaryExpr
  CreateBinaryExpr(BinaryExpr.Opcode op, Expression e0, Expression e1)
    => new BinaryExpr(op, e0, e1);
  public static BinaryExpr CreateAndExpr(Expression e0, Expression e1)
    => CreateBinaryExpr(BinaryExpr.Opcode.And, e0, e1);
  public static BinaryExpr CreateOrExpr(Expression e0, Expression e1)
    => CreateBinaryExpr(BinaryExpr.Opcode.Or, e0, e1);
  public static BinaryExpr CreateLTExpr(Expression e0, Expression e1)
    => CreateBinaryExpr(BinaryExpr.Opcode.Lt, e0, e1);
  public static BinaryExpr CreateLEExpr(Expression e0, Expression e1)
    => CreateBinaryExpr(BinaryExpr.Opcode.Le, e0, e1);
  public static BinaryExpr CreateInExpr(Expression e0, Expression e1)
    => CreateBinaryExpr(BinaryExpr.Opcode.In, e0, e1);

  private static UnaryExpr
  CreateUnaryExpr(UnaryExpr.Opcode op, Expression e)
    => new UnaryExpr(op, e);
  public static UnaryExpr CreateCardinalityExpr(Expression e)
    => CreateUnaryExpr(UnaryExpr.Opcode.Cardinality, e);

  public static MemberSelectExpr
  CreateDatatypeConstructorCheck(Expression e, DatatypeConstructorDecl c) {
    var check = new MemberSelectExpr(e, c.GetDiscriminator());
    check.Type = Type.Bool;
    return check;
  }

}