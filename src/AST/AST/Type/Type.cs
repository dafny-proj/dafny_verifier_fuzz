namespace AST;

public abstract class Type : Node, ConstructableFromDafny<Dafny.Type, Type> {
  public abstract string Name { get; }

  // Instance types.
  public static readonly BoolType Bool = BoolType.Instance;
  public static readonly IntType Int = IntType.Instance;
  public static readonly NatType Nat = NatType.Instance;
  public static readonly StringType String = StringType.Instance;
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

  private static Type UserDefinedTypeHelper(Dafny.UserDefinedType udt) {
    if (udt.Name == "nat") {
      return Type.Nat;
    }
    if (udt.IsStringType) {
      return Type.String;
    }
    if (udt.IsArrayType) {
      return ArrayType.FromDafny(udt);
    }
    return Type.Other;
    // throw new NotImplementedException($"{udType.GetType()}");
  }
}
