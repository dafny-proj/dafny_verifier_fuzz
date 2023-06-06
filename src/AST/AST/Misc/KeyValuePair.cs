namespace AST;

public partial class KeyValuePair<K, V> : Node where K : Node where V : Node { }
public partial class ExpressionPair : KeyValuePair<Expression, Expression> { }
public partial class VarExpressionPair : KeyValuePair<BoundVar, Expression> { }
public partial class AssignmentPair : KeyValuePair<Expression, AssignmentRhs> { }
public partial class MatchExprCase : KeyValuePair<Matcher, Expression> { }
public partial class MatchStmtCase : KeyValuePair<Matcher, BlockStmt> { }
public partial class DatatypeUpdatePair :
KeyValuePair<DatatypeDestructorDecl, Expression> { }

public partial class KeyValuePair<K, V> : Node
where K : Node where V : Node {
  public K Key { get; set; }
  public V Value { get; set; }

  public KeyValuePair(K key, V value) {
    Key = key;
    Value = value;
  }

  public override IEnumerable<Node> Children => new Node[] { Key, Value };
}

public partial class ExpressionPair : KeyValuePair<Expression, Expression> {
  public ExpressionPair(Expression key, Expression value)
  : base(key, value) { }
}

public partial class VarExpressionPair : KeyValuePair<BoundVar, Expression> {
  public VarExpressionPair(BoundVar key, Expression value)
  : base(key, value) { }
}

public partial class AssignmentPair : KeyValuePair<Expression, AssignmentRhs> {
  public AssignmentPair(Expression key, AssignmentRhs value)
  : base(key, value) { }
}

public partial class MatchExprCase : KeyValuePair<Matcher, Expression> {
  public MatchExprCase(Matcher match, Expression body) : base(match, body) { }
}

public partial class MatchStmtCase : KeyValuePair<Matcher, BlockStmt> {
  public MatchStmtCase(Matcher match, BlockStmt body)
  : base(match, body) { }
}

public partial class DatatypeUpdatePair :
KeyValuePair<DatatypeDestructorDecl, Expression> {
  public DatatypeUpdatePair(DatatypeDestructorDecl key, Expression value)
  : base(key, value) { }

  public override IEnumerable<Node> Children => new[] { Value };
}
