namespace AST;

public class LocalVariable
: Node, ConstructableFromDafny<Dafny.LocalVariable, LocalVariable> {
  public string Name { get; set; }

  // Variables may be explicitly typed (e.g. var x: int).
  // Note that this deviates from the original Dafny implementation
  // where a proxy type represents the absence of type declaration.
  public Type? ExplicitType { get; set; }

  private LocalVariable(Dafny.LocalVariable localVarDafny) {
    Name = localVarDafny.Name;
    ExplicitType = localVarDafny.IsTypeExplicit
      ? Type.FromDafny(localVarDafny.OptionalType)
      : null;
  }

  public static LocalVariable FromDafny(Dafny.LocalVariable dafnyNode) {
    return new LocalVariable(dafnyNode);
  }
}