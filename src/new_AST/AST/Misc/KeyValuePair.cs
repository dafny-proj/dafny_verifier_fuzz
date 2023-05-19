namespace AST_new;

public partial class KeyValuePair<K, V> : Node { }
public partial class ExpressionPair : KeyValuePair<Expression, Expression> { }
public partial class AssignmentPair : KeyValuePair<Expression, AssignmentRhs> { }

public partial class KeyValuePair<K, V> : Node {
  public K Key { get; }
  public V Value { get; }

  public KeyValuePair(K key, V value) {
    Key = key;
    Value = value;
  }
}

public partial class ExpressionPair : KeyValuePair<Expression, Expression> {
  public ExpressionPair(Expression key, Expression value)
  : base(key, value) { }
}

public partial class AssignmentPair : KeyValuePair<Expression, AssignmentRhs> {
  public AssignmentPair(Expression key, AssignmentRhs value)
  : base(key, value) { }
}
