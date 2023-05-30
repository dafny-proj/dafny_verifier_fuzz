namespace AST;

public static partial class ASTChildReplacementMethods {
  public static void ReplaceChild(this AssignmentPair n, Node child, Node newChild) {
    if (n.Key == child) {
      n.Key = CheckAndCastNewChild<Expression>(n, child, newChild);
    } else if (n.Value == child) {
      n.Value = CheckAndCastNewChild<AssignmentRhs>(n, child, newChild);
    } else {
      throw new ChildNotFoundException(n, child);
    }
  }


  public static void ReplaceChild(this MatchExprCase n, Node child, Node newChild) {
    if (n.Value == child) {
      n.Value = CheckAndCastNewChild<Expression>(n, child, newChild);
    } else {
      // Other children are unlikely to be mutated here, skip for now.
      throw new UnsupportedNodeChildReplacementException(n);
    }
  }

  public static void ReplaceChild(this ExpressionMatcher n, Node child, Node newChild) {
    if (n.E == child) {
      n.E = CheckAndCastNewChild<Expression>(n, child, newChild);
    } else {
      throw new ChildNotFoundException(n, child);
    }
  }

}
