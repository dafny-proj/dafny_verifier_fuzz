namespace AST_new;

public partial class Formal : Node {
  public string Name { get; }
  public Type Type { get; }
  public Expression? DefaultValue { get; }

  public bool HasDefaultValue() => DefaultValue != null;

  public Formal(string name, Type type, Expression? defaultValue = null) {
    Name = name;
    Type = type;
    DefaultValue = defaultValue;
  }
}