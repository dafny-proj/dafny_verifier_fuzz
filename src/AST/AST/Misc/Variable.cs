namespace AST;

public class Formal : Node, ConstructableFromDafny<Dafny.Formal, Formal> {
  public override IEnumerable<Node> Children {
    get {
      if (DefaultValue != null) yield return DefaultValue;
    }
  }

  public string Name { get; set; }
  public Type Type { get; set; }
  public Expression? DefaultValue { get; set; }

  private Formal(Dafny.Formal fd) {
    Name = fd.Name;
    Type = Type.FromDafny(fd.Type);
    DefaultValue = fd.DefaultValue == null ? null
      : Expression.FromDafny(fd.DefaultValue);
  }

  public static Formal FromDafny(Dafny.Formal dafnyNode) {
    return new Formal(dafnyNode);
  }
}

public class BoundVar : Node, ConstructableFromDafny<Dafny.BoundVar, BoundVar> {
  public string Name { get; set; }
  public Type Type { get; set; }

private BoundVar(Dafny.BoundVar bvd) {
  Name = bvd.Name;
  Type = Type.FromDafny(bvd.Type);
}

public static BoundVar FromDafny(Dafny.BoundVar dafnyNode) {
  return new BoundVar(dafnyNode);
}
}

public class LocalVariable
: Node, ConstructableFromDafny<Dafny.LocalVariable, LocalVariable> {
  public string Name { get; set; }

  // Variables may be explicitly typed (e.g. var x: int).
  // Note that this deviates from the original Dafny implementation
  // where a proxy type represents the absence of type declaration.
  public Type? ExplicitType { get; set; }

  private LocalVariable(Dafny.LocalVariable localVarDafny) {
    Name = localVarDafny.Name;
    ExplicitType = localVarDafny.IsTypeExplicit
      ? Type.FromDafny(localVarDafny.OptionalType)
      : null;
  }

  public static LocalVariable FromDafny(Dafny.LocalVariable dafnyNode) {
    return new LocalVariable(dafnyNode);
  }
}