namespace AST;

public class IntType : Type, ConstructableFromDafny<Dafny.IntType, IntType> {
  private static IntType instance = new IntType();
  public static IntType Instance { get => instance; }
  private IntType() { }

  public override string Name { get => "int"; }

  public static IntType FromDafny(Dafny.IntType dafnyNode) {
    return instance;
  }

}

public class BoolType : Type, ConstructableFromDafny<Dafny.BoolType, BoolType> {
  private static BoolType instance = new BoolType();
  public static BoolType Instance { get => instance; }

  private BoolType() { }

  public override string Name { get => "bool"; }

  public static BoolType FromDafny(Dafny.BoolType dafnyNode) {
    return Instance;
  }
}

public class NatType : Type, ConstructableFromDafny<Dafny.UserDefinedType, NatType> {
  private NatType() { }
  public override string Name { get => "nat"; }

  public static NatType FromDafny(Dafny.UserDefinedType dafnyNode) {
    return new NatType();
  }
}