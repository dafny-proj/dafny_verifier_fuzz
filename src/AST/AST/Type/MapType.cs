namespace AST;

public class MapType
: Type, ConstructableFromDafny<Dafny.MapType, MapType> {

  public Type KeyType { get; }
  public Type ValueType { get; }

  public override string Name => $"map<{KeyType.Name}, {ValueType.Name}>";

  public MapType(Type kt, Type vt) {
    KeyType = kt;
    ValueType = vt;
  }

  private MapType(Dafny.MapType mtd)
  : this(Type.FromDafny(mtd.Domain), Type.FromDafny(mtd.Range)) { }

  public static MapType FromDafny(Dafny.MapType dafnyNode) {
    return new MapType(dafnyNode);
  }
}