namespace AST;

public class NameSegment
: Expression, ConstructableFromDafny<Dafny.NameSegment, NameSegment> {
  public string Name { get; set; }
  private Type _Type;
  public override Type Type { get => _Type; }

  public NameSegment(string name, Type type) {
    Name = name;
    _Type = type;
  }

  private NameSegment(Dafny.NameSegment nameSegmentDafny) {
    Name = nameSegmentDafny.Name;
    _Type = Type.FromDafny(nameSegmentDafny.Type);
  }
  public static NameSegment FromDafny(Dafny.NameSegment dafnyNode) {
    return new NameSegment(dafnyNode);
  }

  public override Expression Clone() {
    // TODO: clone type?
    return new NameSegment(Name, Type);
  }
}
