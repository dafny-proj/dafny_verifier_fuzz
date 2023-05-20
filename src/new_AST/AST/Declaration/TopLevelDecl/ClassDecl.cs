namespace AST_new;

public partial class ClassDecl : TopLevelDecl { }
public partial class DefaultClassDecl : ClassDecl { }
public partial class ArrayClassDecl : ClassDecl { }

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
  public int Dimensions { get; }

  public static string ArrayName(int dimensions) {
    return $"array{(dimensions <= 1 ? "" : dimensions)}";
  }

  public ArrayClassDecl(int dimensions) : base(ArrayName(dimensions)) {
    Dimensions = dimensions;
    // TODO: Add built-in methods.
  }

  public static ArrayClassDecl Skeleton(int dims) => new ArrayClassDecl(dims);
}
