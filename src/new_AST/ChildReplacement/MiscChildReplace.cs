namespace AST_new;

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

}
