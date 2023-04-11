namespace AST;

public abstract class Type : Node, ConstructableFromDafny<Dafny.Type, Type> {
  public abstract string Name { get; }
  public static Type FromDafny(Dafny.Type dafnyNode) {
    return dafnyNode switch {
      Dafny.IntType intType => IntType.FromDafny(intType),
      _ => throw new NotImplementedException(),
    };
  }
}

public class IntType : Type, ConstructableFromDafny<Dafny.IntType, IntType> {
  private IntType() { }

  public override string Name { get => "int"; }

  public static IntType FromDafny(Dafny.IntType dafnyNode) {
    return new IntType();
  }
}