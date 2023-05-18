namespace AST_new;

// Built-in array class.
public partial class ArrayClassDecl : ClassDecl {
  public int Dims { get; }

  public static string ArrayName(int dims) {
    return $"array{(dims <= 1 ? "" : dims)}";
  }

  public ArrayClassDecl(int dims) : base(ArrayName(dims)) {
    Dims = dims;
    // TODO: Add built-in methods.
  }

  public static ArrayClassDecl Skeleton(int dims) => new ArrayClassDecl(dims);
}