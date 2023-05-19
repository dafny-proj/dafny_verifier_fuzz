namespace AST_new;

public abstract partial class Variable {
  public virtual string Name { get; protected set; }
  public virtual Type Type { get; protected set; }

  public Variable(string name, Type type) {
    Name = name;
    Type = type;
  }
}

public partial class BoundVar : Variable {
  public BoundVar(string name, Type type) : base(name, type) { }
}

public partial class Formal : Variable {
  public Expression? DefaultValue { get; }
  public bool HasDefaultValue() => DefaultValue != null;

  public Formal(string name, Type type, Expression? defaultValue = null)
  : base(name, type) {
    DefaultValue = defaultValue;
  }
}