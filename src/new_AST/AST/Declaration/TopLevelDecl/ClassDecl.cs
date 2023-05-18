namespace AST_new;

public partial class ClassDecl : TopLevelDecl {
  public override string Name { get; protected set; }
  public readonly List<MemberDecl> Members = new();

  public ClassDecl(string name, IEnumerable<MemberDecl>? members = null) {
    Name = name;
    if (members != null) {
      Members.AddRange(members);
    }
  }

  public static ClassDecl Skeleton(string name) => new ClassDecl(name);
  public void AddMember(MemberDecl member) => Members.Add(member);
  public void AddMembers(IEnumerable<MemberDecl> members)
    => Members.AddRange(members);
}

public partial class DefaultClassDecl : ClassDecl {
  public DefaultClassDecl(IEnumerable<MemberDecl>? members = null)
  : base("_default_class", members) { }

  public static DefaultClassDecl Skeleton() => new DefaultClassDecl();
}

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