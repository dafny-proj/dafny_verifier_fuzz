using System.Diagnostics.Contracts;

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
    if (udType.IsArrayType) {
      return ArrayType.FromDafny(udType);
    }
    throw new NotImplementedException();
  }
}

public class ArrayType
: Type, ConstructableFromDafny<Dafny.UserDefinedType, ArrayType> {
  public override string Name {
    get {
      var TypeArgsAsStr = string.Join(", ", TypeArgs.Select(t => t.Name));
      return $"{BaseName}<{TypeArgsAsStr}>";
    }
  }
  public string BaseName { get; }
  
  // TODO: ArrayTypes should only have 1 type argument? Consider replacing 
  // List<Type> with Type or extract the following fields to a class for generic types.
  public List<Type>? ProvidedTypeArgs;
  public List<Type> TypeArgs = new List<Type>();

  private ArrayType(Dafny.UserDefinedType udtDafny) {
    Contract.Assert(udtDafny.IsArrayType);
    BaseName = udtDafny.Name;
    TypeArgs.AddRange(udtDafny.TypeArgs.Select(Type.FromDafny));
    if (udtDafny.NamePath is Dafny.NameSegment ns) {
      ProvidedTypeArgs = ns.OptTypeArguments.Select(Type.FromDafny).ToList();
    } else {
      throw new NotImplementedException();
    }
  }

  public static ArrayType FromDafny(Dafny.UserDefinedType dafnyNode) {
    return new ArrayType(dafnyNode);
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