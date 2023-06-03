namespace AST;

public static partial class ASTChildReplacementMethods {
  public static void ReplaceChild(this Declaration n, Node child, Node newChild) {
    switch (n) {
      case FunctionDecl d:
        d.ReplaceChild(child, newChild);
        break;
      default:
        throw new UnsupportedNodeChildReplacementException(n);
    }
  }

  public static void ReplaceChild(this FunctionDecl n, Node child, Node newChild) {
    if (n.Body == child) {
      n.Body = CheckAndCastNewChild<Expression>(n, child, newChild);
    } else {
      // Other children are unlikely to be mutated here, skip for now.
      throw new UnsupportedNodeChildReplacementException(n);
    }
  }
}
