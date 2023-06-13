namespace AST;

public abstract partial class TopLevelDecl : Declaration {
  public static readonly TopLevelDecl BuiltIn = new BuiltInDecl();
  // Dafny contains e.g. a ValueTypeDecl for handling built in members of 
  // primitives e.g. map.Keys. A more appropriate solution is to create 
  // declarations to back-up primitive types. For now, just use a singleton
  // catch-all declaration.
  private class BuiltInDecl : TopLevelDecl {
    public override IEnumerable<Node> Children
      => Enumerable.Empty<Node>();
  }
}