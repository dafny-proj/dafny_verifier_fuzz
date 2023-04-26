using System.Diagnostics.Contracts;

namespace AST;

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