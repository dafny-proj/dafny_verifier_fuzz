namespace AST;

public abstract partial class Variable : Node { }
public partial class LocalVar : Variable { }
public partial class BoundVar : Variable { }
public partial class Formal : Variable { }


public abstract partial class Variable : Node {
  public virtual string Name { get; protected set; }
  public virtual Type Type { get; protected set; }
  public virtual Type? ExplicitType { get; protected set; }

  public bool HasExplicitType() => ExplicitType != null;

  public Variable(string name, Type type, Type? explicitType = null) {
    Name = name;
    Type = type;
    ExplicitType = explicitType;
  }

  public override IEnumerable<Node> Children
    => HasExplicitType() ? new[] { ExplicitType! } : Enumerable.Empty<Node>();
}

public partial class LocalVar : Variable {
  public LocalVar(string name, Type type, Type? explicitType = null)
  : base(name, type, explicitType) { }
}

public partial class BoundVar : Variable {
  public BoundVar(string name, Type type, Type? explicitType = null)
  : base(name, type, explicitType) { }
}

public partial class Formal : Variable {
  public Expression? DefaultValue { get; }
  public bool HasDefaultValue() => DefaultValue != null;

  // Formals always have explicit types.
  public Formal(string name, Type type, Expression? defaultValue = null)
  : base(name, type, explicitType: type) {
    DefaultValue = defaultValue;
  }

  public override IEnumerable<Node> Children
    => HasDefaultValue() ? base.Children.Append(DefaultValue!) : base.Children;
}