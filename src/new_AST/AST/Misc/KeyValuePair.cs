namespace AST_new;

public partial class KeyValuePair : Node {
  public Expression E0 { get; }
  public Expression E1 { get; }

  public KeyValuePair(Expression key, Expression value) {
    E0 = key;
    E1 = value;
  }
}
