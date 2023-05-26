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

  public override bool Equals(object? obj) {
    if (obj == null || obj is not UserDefinedType) { return false; }
    var other = (UserDefinedType)obj;
    if (this.TypeDecl != other.TypeDecl) { return false; }
    if (this.TypeArgs.Count != other.TypeArgs.Count) { return false; }
    foreach (var (a1, a2) in this.TypeArgs.Zip(other.TypeArgs)) {
      if (!a1.Equals(a2)) { return false; }
    }
    return true;
  }

  public override int GetHashCode() {
    int h = TypeDecl.GetHashCode();
    foreach (var t in TypeArgs) {
      h += t.GetHashCode();
    }
    return h;
  }
}
