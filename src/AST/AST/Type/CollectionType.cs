namespace AST;

public abstract partial class CollectionType : Type { }
public partial class MapType : CollectionType { }
public partial class SeqType : CollectionType { }
public partial class SetType : CollectionType { }
public partial class MultiSetType : CollectionType { }

public partial class MapType : CollectionType {
  public override string BaseName => "map";
  public Type KeyType { get; }
  public Type ValueType { get; }

  public MapType(Type keyType, Type valueType) {
    KeyType = keyType;
    ValueType = valueType;
    TypeArgs.AddRange(new[] { keyType, valueType });
  }
}

public partial class SeqType : CollectionType {
  public override string BaseName => "seq";
  public Type ElementType { get; }

  public SeqType(Type elementType) {
    ElementType = elementType;
    TypeArgs.Add(elementType);
  }
}

public partial class SetType : CollectionType {
  public override string BaseName => "set";
  public Type ElementType { get; }

  public SetType(Type elementType) {
    ElementType = elementType;
    TypeArgs.Add(elementType);
  }
}

public partial class MultiSetType : CollectionType {
  public override string BaseName => "multiset";
  public Type ElementType { get; }

  public MultiSetType(Type elementType) {
    ElementType = elementType;
    TypeArgs.Add(elementType);
  }
}
