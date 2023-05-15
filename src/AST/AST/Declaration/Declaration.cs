namespace AST;

public class Declaration
: Node, ConstructableFromDafny<Dafny.Declaration, Declaration> {
  public static Declaration FromDafny(Dafny.Declaration dafnyNode) {
    return dafnyNode switch {
      Dafny.TopLevelDecl topLevelDecl
        => TopLevelDecl.FromDafny(topLevelDecl),
      Dafny.DatatypeCtor datatypeCtor
        => DatatypeConstructor.FromDafny(datatypeCtor),
      _ => throw new NotImplementedException(),
    };
  }
}
