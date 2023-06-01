namespace AST;

public static partial class ASTChildReplacementMethods {
  public static void ReplaceChild(this Node node, Node child, Node newChild) {
    switch (node) {
      case Expression n:
        n.ReplaceChild(child, newChild);
        break;
      case Statement n:
        n.ReplaceChild(child, newChild);
        break;
      case AssignmentRhs n:
        n.ReplaceChild(child, newChild);
        break;
      case AssignmentPair n:
        n.ReplaceChild(child, newChild);
        break;
      case ExpressionPair n:
        n.ReplaceChild(child, newChild);
        break;
      case VarExpressionPair n:
        n.ReplaceChild(child, newChild);
        break;
      case MatchExprCase n:
        n.ReplaceChild(child, newChild);
        break;
      case DatatypeUpdatePair n:
        n.ReplaceChild(child, newChild);
        break;
      case Matcher n:
        n.ReplaceChild(child, newChild);
        break;
      case Specification n:
        n.ReplaceChild(child, newChild);
        break;
      default:
        throw new UnsupportedNodeChildReplacementException(node);
    }
  }

  // Replace `child` in `children` with `newChild`.
  private static bool
  ReplaceInList<T>(List<T> children, Node child, Node newChild, Node parent, bool end = true)
  where T : Node {
    if (newChild is not T) {
      throw new ChildReplaceIncompatibleTypeException(parent, child, newChild);
    }
    var index = children.FindIndex(c => c == child);
    if (index == -1) {
      if (end) {
        throw new ChildNotFoundException(parent, child);
      } else {
        return false;
      }
    }
    children[index] = (T)newChild;
    return true;
  }

  private static T
  CheckAndCastNewChild<T>(Node parent, Node child, Node newChild) where T : Node {
    if (newChild is not T) {
      throw new ChildReplaceIncompatibleTypeException(parent, child, newChild);
    }
    return (T)newChild;
  }

}