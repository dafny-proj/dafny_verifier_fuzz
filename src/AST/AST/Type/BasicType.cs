namespace AST;

public class IntType : Type, ConstructableFromDafny<Dafny.IntType, IntType> {
  public override string Name => "int";
  public static IntType Instance => instance;

  private static IntType instance = new IntType();
  private IntType() { }

  public static IntType FromDafny(Dafny.IntType dafnyNode) {
    return Instance;
  }
}

public class BoolType : Type, ConstructableFromDafny<Dafny.BoolType, BoolType> {
  public override string Name => "bool";
  public static BoolType Instance => instance;

  private static BoolType instance = new BoolType();
  private BoolType() { }

  public static BoolType FromDafny(Dafny.BoolType dafnyNode) {
    return Instance;
  }
}

public class NatType : Type, ConstructableFromDafny<Dafny.UserDefinedType, NatType> {
  public override string Name => "nat";
  public static NatType Instance => instance;

  private static NatType instance = new NatType();
  private NatType() { }

  public static NatType FromDafny(Dafny.UserDefinedType dafnyNode) {
    return Instance;
  }
}

public class StringType
: Type, ConstructableFromDafny<Dafny.UserDefinedType, StringType> {
  public override string Name => "string";
  public static StringType Instance => instance;

  private static StringType instance = new StringType();
  private StringType() { }

  public static StringType FromDafny(Dafny.UserDefinedType dafnyNode) {
    return Instance;
  }
}