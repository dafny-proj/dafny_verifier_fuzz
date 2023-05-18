namespace AST_new;

public partial class UserDefinedType : Type {
  public override string BaseName => TypeDecl.Name;

  public TopLevelDecl TypeDecl { get; }

  public UserDefinedType(TopLevelDecl typeDecl,
  IEnumerable<Type>? typeArgs = null) {
    TypeDecl = typeDecl;
    if (typeArgs != null) {
      TypeArgs.AddRange(typeArgs);
    }
  }
}
