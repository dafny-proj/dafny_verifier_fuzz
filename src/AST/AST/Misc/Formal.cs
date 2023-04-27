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