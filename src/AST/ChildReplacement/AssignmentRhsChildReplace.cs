namespace AST;

public static partial class ASTChildReplacementMethods {
  public static void ReplaceChild(this AssignmentRhs n, Node child, Node newChild) {
    switch (n) {
      case ExprRhs a:
        a.ReplaceChild(child, newChild);
        break;
      case NewArrayRhs a:
        a.ReplaceChild(child, newChild);
        break;
      case NewObjectWithConstructorRhs a:
        a.ReplaceChild(child, newChild);
        break;
      case MethodCallRhs a:
        a.ReplaceChild(child, newChild);
        break;
      default:
        throw new UnsupportedNodeChildReplacementException(n);
    }
  }

  public static void ReplaceChild(this ExprRhs n, Node child, Node newChild) {
    if (n.E == child) {
      n.E = CheckAndCastNewChild<Expression>(n, child, newChild);
    } else {
      throw new ChildNotFoundException(n, child);
    }
  }

  public static void ReplaceChild(this NewArrayRhs n, Node child, Node newChild) {
    var replaced = ReplaceInList<Expression>(n.Dimensions, child, newChild, n, end: false);
    if (replaced) { return; }
    if (n is NewArrayWithElementInitialiserRhs e) {
      if (e.ElementInitialiser == child) {
        e.ElementInitialiser = CheckAndCastNewChild<Expression>(n, child, newChild);
        return;
      }
    } else if (n is NewArrayWithListInitialiserRhs l) {
      ReplaceInList<Expression>(l.ListInitialiser, child, newChild, n);
      return;
    }
    throw new ChildNotFoundException(n, child);
  }

  public static void ReplaceChild(this NewObjectWithConstructorRhs n, Node child, Node newChild) {
    ReplaceInList<Expression>(n.ConstructorArguments, child, newChild, n);
  }

  public static void ReplaceChild(this MethodCallRhs n, Node child, Node newChild) {
    if (n.Callee == child) {
      n.Callee = CheckAndCastNewChild<MemberSelectExpr>(n, child, newChild);
    } else {
      ReplaceInList<Expression>(n.Arguments, child, newChild, n);
    }
  }

}
