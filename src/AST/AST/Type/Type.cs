namespace AST;

public abstract class Type : Node, ConstructableFromDafny<Dafny.Type, Type> {
  public abstract string Name { get; }

  public static readonly BoolType Bool = BoolType.Instance;
  public static readonly IntType Int = IntType.Instance;
  public static readonly OtherType Other = OtherType.Instance;

  public static Type FromDafny(Dafny.Type dafnyNode) {
    return dafnyNode switch {
      Dafny.IntType intType => Type.Int,
      Dafny.BoolType boolType => Type.Bool,
      Dafny.UserDefinedType udType => UserDefinedTypeHelper(udType),
      _ => Type.Other,
      // _ => throw new NotImplementedException($"{dafnyNode.GetType()}"),
    };
  }

  private static Type UserDefinedTypeHelper(Dafny.UserDefinedType udType) {
    if (udType.Name == "nat") {
      return NatType.FromDafny(udType);
    }
    if (udType.IsArrayType) {
      return ArrayType.FromDafny(udType);
    }
    return Type.Other;
    // throw new NotImplementedException($"{udType.GetType()}");
  }
}
