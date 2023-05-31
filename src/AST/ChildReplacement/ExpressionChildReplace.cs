namespace AST;

public static partial class ASTChildReplacementMethods {
  public static void ReplaceChild(this Expression n, Node child, Node newChild) {
    switch (n) {
      case AutoGeneratedExpr e:
        e.ReplaceChild(child, newChild);
        break;
      case ParensExpr e:
        e.ReplaceChild(child, newChild);
        break;
      case BinaryExpr e:
        e.ReplaceChild(child, newChild);
        break;
      case UnaryExpr e:
        e.ReplaceChild(child, newChild);
        break;
      case MemberSelectExpr e:
        e.ReplaceChild(child, newChild);
        break;
      case CollectionSelectExpr e:
        e.ReplaceChild(child, newChild);
        break;
      case SeqDisplayExpr e:
        e.ReplaceChild(child, newChild);
        break;
      case SetDisplayExpr e:
        e.ReplaceChild(child, newChild);
        break;
      case MultiSetDisplayExpr e:
        e.ReplaceChild(child, newChild);
        break;
      case MapDisplayExpr e:
        e.ReplaceChild(child, newChild);
        break;
      case CollectionUpdateExpr e:
        e.ReplaceChild(child, newChild);
        break;
      case MultiSetConstructionExpr e:
        e.ReplaceChild(child, newChild);
        break;
      case SeqConstructionExpr e:
        e.ReplaceChild(child, newChild);
        break;
      case DatatypeValueExpr e:
        e.ReplaceChild(child, newChild);
        break;
      case FunctionCallExpr e:
        e.ReplaceChild(child, newChild);
        break;
      case ITEExpr e:
        e.ReplaceChild(child, newChild);
        break;
      case LetExpr e:
        e.ReplaceChild(child, newChild);
        break;
      case MatchExpr e:
        e.ReplaceChild(child, newChild);
        break;
      case LambdaExpr e:
        e.ReplaceChild(child, newChild);
        break;
      case SetComprehensionExpr e:
        e.ReplaceChild(child, newChild);
        break;
      case MapComprehensionExpr e:
        e.ReplaceChild(child, newChild);
        break;
      default:
        throw new UnsupportedNodeChildReplacementException(n);
    }
  }

  public static void ReplaceChild(this AutoGeneratedExpr n, Node child, Node newChild) {
    if (n.E == child) {
      n.E = CheckAndCastNewChild<Expression>(n, child, newChild);
    } else {
      throw new ChildNotFoundException(n, child);
    }
  }

  public static void ReplaceChild(this ParensExpr n, Node child, Node newChild) {
    if (n.E == child) {
      n.E = CheckAndCastNewChild<Expression>(n, child, newChild);
    } else {
      throw new ChildNotFoundException(n, child);
    }
  }

  public static void ReplaceChild(this BinaryExpr n, Node child, Node newChild) {
    var newE = CheckAndCastNewChild<Expression>(n, child, newChild);
    if (n.E0 == child) {
      n.E0 = newE;
    } else if (n.E1 == child) {
      n.E1 = newE;
    } else {
      throw new ChildNotFoundException(n, child);
    }
  }

  public static void ReplaceChild(this UnaryExpr n, Node child, Node newChild) {
    if (n.E == child) {
      n.E = CheckAndCastNewChild<Expression>(n, child, newChild);
    } else {
      throw new ChildNotFoundException(n, child);
    }
  }

  public static void ReplaceChild(this MemberSelectExpr n, Node child, Node newChild) {
    if (n.Receiver == child) {
      n.Receiver = CheckAndCastNewChild<Expression>(n, child, newChild);
    } else {
      throw new UnsupportedNodeChildReplacementException(n);
    }
  }

  public static void ReplaceChild(this CollectionSelectExpr n, Node child, Node newChild) {
    var newE = CheckAndCastNewChild<Expression>(n, child, newChild); ;
    if (n.Collection == child) {
      n.Collection = newE;
      return;
    }
    if (n is CollectionElementExpr e) {
      if (e.Index == child) {
        e.Index = newE;
        return;
      }
    } else if (n is CollectionSliceExpr s) {
      if (s.Index0 == child) {
        s.Index0 = newE;
        return;
      } else if (s.Index1 == child) {
        s.Index1 = newE;
        return;
      }
    }
    throw new ChildNotFoundException(n, child);
  }

  public static void ReplaceChild(this SeqDisplayExpr n, Node child, Node newChild) {
    ReplaceInList<Expression>(n.Elements, child, newChild, n);
  }
  public static void ReplaceChild(this SetDisplayExpr n, Node child, Node newChild) {
    ReplaceInList<Expression>(n.Elements, child, newChild, n);
  }
  public static void ReplaceChild(this MultiSetDisplayExpr n, Node child, Node newChild) {
    ReplaceInList<Expression>(n.Elements, child, newChild, n);
  }
  public static void ReplaceChild(this MapDisplayExpr n, Node child, Node newChild) {
    ReplaceInList<ExpressionPair>(n.Elements, child, newChild, n);
  }

  public static void ReplaceChild(this CollectionUpdateExpr n, Node child, Node newChild) {
    var newE = CheckAndCastNewChild<Expression>(n, child, newChild);
    if (n.Collection == child) {
      n.Collection = newE;
    } else if (n.Index == child) {
      n.Index = newE;
    } else if (n.Value == child) {
      n.Value = newE;
    } else {
      throw new ChildNotFoundException(n, child);
    }
  }

  public static void ReplaceChild(this MultiSetConstructionExpr n, Node child, Node newChild) {
    if (n.E == child) {
      n.E = CheckAndCastNewChild<Expression>(n, child, newChild);
    } else {
      throw new ChildNotFoundException(n, child);
    }
  }

  public static void ReplaceChild(this SeqConstructionExpr n, Node child, Node newChild) {
    if (n.Count == child) {
      n.Count = CheckAndCastNewChild<Expression>(n, child, newChild);
    } else if (n.Initialiser == child) {
      n.Initialiser = CheckAndCastNewChild<Expression>(n, child, newChild);
    } else {
      throw new ChildNotFoundException(n, child);
    }
  }

  public static void ReplaceChild(this DatatypeValueExpr n, Node child, Node newChild) {
    ReplaceInList<Expression>(n.ConstructorArguments, child, newChild, n);
  }

  public static void ReplaceChild(this FunctionCallExpr n, Node child, Node newChild) {
    if (n.Callee == child) {
      n.Callee = CheckAndCastNewChild<MemberSelectExpr>(n, child, newChild);
    } else {
      ReplaceInList<Expression>(n.Arguments, child, newChild, n);
    }
  }

  public static void ReplaceChild(this ITEExpr n, Node child, Node newChild) {
    var newE = CheckAndCastNewChild<Expression>(n, child, newChild);
    if (n.Guard == child) {
      n.Guard = newE;
    } else if (n.Thn == child) {
      n.Thn = newE;
    } else if (n.Els == child) {
      n.Els = newE;
    } else {
      throw new ChildNotFoundException(n, child);
    }
  }

  public static void ReplaceChild(this LetExpr n, Node child, Node newChild) {
    if (n.Body == child) {
      n.Body = CheckAndCastNewChild<Expression>(n, child, newChild);
    } else {
      // Other children are unlikely to be mutated here, skip for now.
      throw new UnsupportedNodeChildReplacementException(n);
    }
  }

  public static void ReplaceChild(this MatchExpr n, Node child, Node newChild) {
    if (n.Selector == child) {
      n.Selector = CheckAndCastNewChild<Expression>(n, child, newChild);
    } else {
      // Other children are unlikely to be mutated here, skip for now.
      throw new UnsupportedNodeChildReplacementException(n);
    }
  }

  public static void ReplaceChild(this LambdaExpr n, Node child, Node newChild) {
    if (n.Result == child) {
      n.Result = CheckAndCastNewChild<Expression>(n, child, newChild);
    } else {
      // Other children are unlikely to be mutated here, skip for now.
      throw new UnsupportedNodeChildReplacementException(n);
    }
  }

  public static void ReplaceChild(this DatatypeUpdateExpr n, Node child, Node newChild) {
    if (n.DatatypeValue == child) {
      n.DatatypeValue = CheckAndCastNewChild<Expression>(n, child, newChild);
    } else {
      // Other children are unlikely to be mutated here, skip for now.
      throw new UnsupportedNodeChildReplacementException(n);
    }
  }

  public static void ReplaceChild(this SetComprehensionExpr n, Node child, Node newChild) {
    if (n.Value == child) {
      n.Value = CheckAndCastNewChild<Expression>(n, child, newChild);
    } else {
      // Other children are unlikely to be mutated here, skip for now.
      throw new UnsupportedNodeChildReplacementException(n);
    }
  }

  public static void ReplaceChild(this MapComprehensionExpr n, Node child, Node newChild) {
    if (n.Key == child) {
      n.Key = CheckAndCastNewChild<Expression>(n, child, newChild);
    } else if (n.Value == child) {
      n.Value = CheckAndCastNewChild<Expression>(n, child, newChild);
    } else {
      // Other children are unlikely to be mutated here, skip for now.
      throw new UnsupportedNodeChildReplacementException(n);
    }
  }

}
