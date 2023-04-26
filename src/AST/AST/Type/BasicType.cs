namespace AST;

public class IntType : Type, ConstructableFromDafny<Dafny.IntType, IntType> {
  private IntType() { }

  public override string Name { get => "int"; }

  public static IntType FromDafny(Dafny.IntType dafnyNode) {
    return new IntType();
  }
}

public class BoolType : Type, ConstructableFromDafny<Dafny.BoolType, BoolType> {
  private BoolType() { }

  public override string Name { get => "bool"; }

  public static BoolType FromDafny(Dafny.BoolType dafnyNode) {
    return new BoolType();
  }
}

public class NatType : Type, ConstructableFromDafny<Dafny.UserDefinedType, NatType> {
  private NatType() { }
  public override string Name { get => "nat"; }

  public static NatType FromDafny(Dafny.UserDefinedType dafnyNode) {
    return new NatType();
  }
}