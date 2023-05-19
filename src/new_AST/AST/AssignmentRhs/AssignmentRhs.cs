namespace AST_new;

public partial class ExprRhs : AssignmentRhs {
  public ExprRhs(Expression e) {
    // TODO
  }
}

public partial class MethodCallRhs : AssignmentRhs {
  public MethodCallRhs(MemberSelectExpr callee,
  IEnumerable<Expression>? arguments = null) {
    // TODO
  }
}
