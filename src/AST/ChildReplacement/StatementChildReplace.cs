namespace AST;

public static partial class ASTChildReplacementMethods {
  public static void ReplaceChild(this Statement n, Node child, Node newChild) {
    switch (n) {
      case BlockStmt s:
        s.ReplaceChild(child, newChild);
        break;
      case AssignStmt s:
        s.ReplaceChild(child, newChild);
        break;
      case CallStmt s:
        s.ReplaceChild(child, newChild);
        break;
      case PrintStmt s:
        s.ReplaceChild(child, newChild);
        break;
      case IfStmt s:
        s.ReplaceChild(child, newChild);
        break;
      case WhileLoopStmt s:
        s.ReplaceChild(child, newChild);
        break;
      case ForLoopStmt s:
        s.ReplaceChild(child, newChild);
        break;
      case AssertStmt s:
        s.ReplaceChild(child, newChild);
        break;
      case MatchStmt s:
        s.ReplaceChild(child, newChild);
        break;
      case ExpectStmt s:
        s.ReplaceChild(child, newChild);
        break;
      default:
        throw new UnsupportedNodeChildReplacementException(n);
    }
  }

  public static void ReplaceChild(this BlockStmt n, Node child, Node newChild) {
    ReplaceInList<Statement>(n.Body, child, newChild, n);
  }

  public static void ReplaceChild(this AssignStmt n, Node child, Node newChild) {
    ReplaceInList<AssignmentPair>(n.Assignments, child, newChild, n);
  }

  public static void ReplaceChild(this CallStmt n, Node child, Node newChild) {
    if (n.Call == child) {
      n.Call = CheckAndCastNewChild<MethodCallRhs>(n, child, newChild);
    } else {
      ReplaceInList<Expression>(n.CallLhss, child, newChild, n);
    }
  }

  public static void ReplaceChild(this PrintStmt n, Node child, Node newChild) {
    ReplaceInList<Expression>(n.Expressions, child, newChild, n);
  }

  public static void ReplaceChild(this IfStmt n, Node child, Node newChild) {
    if (n.Guard == child) {
      n.Guard = CheckAndCastNewChild<Expression>(n, child, newChild);
    } else if (n.Thn == child) {
      n.Thn = CheckAndCastNewChild<BlockStmt>(n, child, newChild);
    } else if (n.Els == child) {
      n.Els = CheckAndCastNewChild<Statement>(n, child, newChild);
    } else {
      throw new ChildNotFoundException(n, child);
    }
  }

  public static void ReplaceChild(this WhileLoopStmt n, Node child, Node newChild) {
    if (n.Guard == child) {
      n.Guard = CheckAndCastNewChild<Expression>(n, child, newChild);
    } else {
      // Other references are unlikely to be updated. Skip for now.
      throw new UnsupportedNodeChildReplacementException(n);
    }
  }

  public static void ReplaceChild(this ForLoopStmt n, Node child, Node newChild) {
    if (n.LoopStart == child) {
      n.LoopStart = CheckAndCastNewChild<Expression>(n, child, newChild);
    } else if (n.LoopEnd == child) {
      n.LoopEnd = CheckAndCastNewChild<Expression>(n, child, newChild);
    } else {
      // Other references are unlikely to be updated. Skip for now.
      throw new UnsupportedNodeChildReplacementException(n);
    }
  }

  public static void ReplaceChild(this AssertStmt n, Node child, Node newChild) {
    if (n.Assertion == child) {
      n.Assertion = CheckAndCastNewChild<Expression>(n, child, newChild);
    } else {
      throw new ChildNotFoundException(n, child);
    }
  }

  public static void ReplaceChild(this MatchStmt n, Node child, Node newChild) {
    if (n.Selector == child) {
      n.Selector = CheckAndCastNewChild<Expression>(n, child, newChild);
    } else {
      // Other children are unlikely to be mutated here, skip for now.
      throw new UnsupportedNodeChildReplacementException(n);
    }
  }

  public static void ReplaceChild(this ExpectStmt n, Node child, Node newChild) {
    if (n.Expectation == child) {
      n.Expectation = CheckAndCastNewChild<Expression>(n, child, newChild);
    } else {
      throw new ChildNotFoundException(n, child);
    }
  }

}
