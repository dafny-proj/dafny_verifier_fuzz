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
      Dafny.MapType mapType => MapType.FromDafny(mapType),
      Dafny.UserDefinedType udType => UserDefinedType.FromDafny(udType),
      _ => Type.Other,
      // _ => throw new NotImplementedException($"{dafnyNode.GetType()}"),
    };
  }
}

public class UserDefinedType
: Type, ConstructableFromDafny<Dafny.UserDefinedType, Type> {
  private Dafny.UserDefinedType? DafnyType { get; }
  private string _Name { get; }
  public override string Name {
    get {
      var name = _Name;
      if (TypeArgs.Count > 0) {
        name += "<";
        name += string.Join(", ", TypeArgs.Select(t => t.Name));
        name += ">";
      }
      return name;
    }
  }
  public List<Type> TypeArgs = new();

  public UserDefinedType(string name, List<Type>? typeArgs = null) {
    _Name = name;
    if (typeArgs != null) {
      TypeArgs.AddRange(typeArgs);
    }
  }

  private UserDefinedType(Dafny.UserDefinedType udt) {
    _Name = udt.Name;
    DafnyType = udt;
    TypeArgs.AddRange(udt.TypeArgs.Select(Type.FromDafny));
  }

  public static Type FromDafny(Dafny.UserDefinedType udt) {
    if (udt.Name == "nat") {
      return Type.Nat;
    }
    if (udt.IsStringType) {
      return Type.String;
    }
    if (udt.IsArrayType) {
      return ArrayType.FromDafny(udt);
    }
    return new UserDefinedType(udt);
    // throw new NotImplementedException($"{udt.GetType()}");
  }
}
