namespace AST_new;

public abstract partial class BuiltInType : UserDefinedType {
  protected BuiltInType(TopLevelDecl typeDecl,
  IEnumerable<Type>? typeArgs = null) : base(typeDecl, typeArgs) { }
}

public partial class NatType : BuiltInType {
  public readonly static NatType Instance = new NatType();

  private static SubsetTypeDecl NatDecl = new SubsetTypeDecl(
    name: "nat",
    baseIdent: new BoundVar("x", Type.Int),
    constraint: new BoolLiteralExpr(true));
  // FIXME: Correct constraint should be x >= 0.

  private NatType() : base(typeDecl: NatDecl) { }
}

public partial class StringType : BuiltInType {
  public readonly static StringType Instance = new StringType();

  private static TypeSynonymDecl _stringDecl
    = new TypeSynonymDecl(name: "string", baseType: new SeqType(Type.Char));

  private StringType() : base(typeDecl: _stringDecl) { }
}

public partial class ArrayType : BuiltInType {
  public Type ElementType { get; }
  public ArrayType(ArrayClassDecl arrayClass, Type elementType)
  : base(typeDecl: arrayClass, typeArgs: new[] { elementType }) {
    ElementType = elementType;
  }
}
