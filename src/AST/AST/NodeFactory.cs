namespace AST;

public static class NodeFactory {
  public static BinaryExpr
  CreateOrExpr(Expression e0, Expression e1) {
    return new BinaryExpr(BinaryExpr.Opcode.Or, e0, e1);
  }

  public static MemberSelectExpr
  CreateDatatypeConstructorCheck(Expression e, DatatypeConstructorDecl c) {
    var check = new MemberSelectExpr(e, c.GetDiscriminator());
    check.Type = Type.Bool;
    return check;
  }
}