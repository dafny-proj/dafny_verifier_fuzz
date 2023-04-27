namespace AST;

// FIXME: This is a temporary fix to handle translation of unresolved Dafny nodes.
// All the translation should be fixed to only work with resolved Dafny nodes, 
// where this class then can be removed.
public class OtherType : Type, ConstructableFromDafny<Dafny.Type, OtherType> {
  private static OtherType _Instance = new OtherType();
  public static OtherType Instance => _Instance;
  public override string Name { get => "other type"; }
  private OtherType() { }
  public new static OtherType FromDafny(Dafny.Type dafnyNode) {
    return Instance;
  }
}