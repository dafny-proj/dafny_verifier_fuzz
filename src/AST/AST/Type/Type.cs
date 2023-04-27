namespace AST;

public abstract class Type : Node, ConstructableFromDafny<Dafny.Type, Type> {
  public abstract string Name { get; }

  public static readonly BoolType Bool = BoolType.Instance;
  public static readonly IntType Int = IntType.Instance; 

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
    if (udType.IsArrayType) {
      return ArrayType.FromDafny(udType);
    }
    throw new NotImplementedException();
  }
}

