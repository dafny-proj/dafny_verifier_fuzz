namespace AST;

public partial class UserDefinedType : Type { }
public partial class ArrowType : UserDefinedType { }

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

public partial class ArrowType : UserDefinedType {
  public FuncT Arrow { get; }
  public string ArrowStr => arrowStr[Arrow];
  public int Arity => TypeArgs.Count - 1;
  public List<Type> ArgTypes => TypeArgs.Take(Arity).ToList();
  public Type ResultType => TypeArgs.Last();

  public enum FuncT { All, Partial, Total }
  private Dictionary<FuncT, string> arrowStr = new() {
    {FuncT.All, "~>"}, {FuncT.Partial, "-->"}, {FuncT.Total, "->"}};

  public ArrowType(FuncT arrow, TopLevelDecl typeDecl,
  IEnumerable<Type>? typeArgs = null)
  : base(typeDecl, typeArgs) {
    Arrow = arrow;
  }
}
