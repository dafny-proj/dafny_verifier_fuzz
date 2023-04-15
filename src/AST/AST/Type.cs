namespace AST;

public abstract class Type : Node, ConstructableFromDafny<Dafny.Type, Type> {
  public abstract string Name { get; }
  public static Type FromDafny(Dafny.Type dafnyNode) {
    return dafnyNode switch {
      Dafny.IntType intType => IntType.FromDafny(intType),
      Dafny.BoolType boolType => BoolType.FromDafny(boolType),
      Dafny.UserDefinedType udType => UserDefinedTypeHelper(udType),
      _ => throw new NotImplementedException(),
    };
  }

  private static Type UserDefinedTypeHelper(Dafny.UserDefinedType udType) {
    if (udType.Name == "nat") {
      return NatType.FromDafny(udType);
    }
    throw new NotImplementedException();
  }
}

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