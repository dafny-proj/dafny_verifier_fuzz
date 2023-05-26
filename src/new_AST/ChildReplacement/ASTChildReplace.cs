namespace AST_new;

public static partial class ASTChildReplacementMethods {
  public static void ReplaceChild(this Node n, Node child, Node newChild) {
    switch (n) {
      case Expression e:
        e.ReplaceChild(child, newChild);
        break;
      case Statement s:
        s.ReplaceChild(child, newChild);
        break;
      case AssignmentRhs a:
        a.ReplaceChild(child, newChild);
        break;
      case AssignmentPair a:
        a.ReplaceChild(child, newChild);
        break;
      default:
        throw new UnsupportedNodeChildReplacementException(n);
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