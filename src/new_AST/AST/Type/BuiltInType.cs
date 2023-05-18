namespace AST_new;

public abstract partial class BuiltInType : UserDefinedType {
  protected BuiltInType(TopLevelDecl typeDecl,
  IEnumerable<Type>? typeArgs = null) : base(typeDecl, typeArgs) { }
}

// TODO: Create subset type.
public partial class NatType : BuiltInType {
  public static NatType Instance => new NatType();
  private NatType() : base(typeDecl: new SubsetTypeDecl("nat")) { }
}

// TODO: Create type synonym seq<char>.
public partial class StringType : BuiltInType {
  public static StringType Instance => new StringType();
  private StringType() : base(typeDecl: new TypeSynonymDecl("string")) { }
}

public partial class ArrayType : BuiltInType {
  public Type ElementType { get; }
  public ArrayType(ArrayClassDecl arrayClass, Type elementType)
  : base(typeDecl: arrayClass, typeArgs: new[] { elementType }) {
    ElementType = elementType;
  }
}
