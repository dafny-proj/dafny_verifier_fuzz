namespace AST;

public partial class ClassDecl : TopLevelDecl { }
public partial class DefaultClassDecl : ClassDecl { }
public partial class ArrayClassDecl : ClassDecl { }
public partial class ArrowTypeDecl : ClassDecl { }

// TODO: Type parameters for classes.
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

  public override IEnumerable<Node> Children => Members;
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
    // TODO: Automatically populate built-in methods. For now, they are 
    // translated and added if present in Dafny programs.
  }

  public static ArrayClassDecl Skeleton(int dims) => new ArrayClassDecl(dims);
}

// TODO: This class inherits from ClassDecl which reflects the implementation in 
// Dafny. Consider removing the inheritance, they don't really correspond.
// Built-in arrow type class. (f: A ~> B). 
// This class gives rise to two subset types (f: A --> B) and (f: A -> B).
public partial class ArrowTypeDecl : ClassDecl {
  // Number of arguments to the function. Excludes the single produced result.
  public int Arity { get; }
  public ArrowTypeDecl(int arity) : base("_DF_builtin_arrow") { }

  public static ArrowTypeDecl Skeleton(int arity) => new ArrowTypeDecl(arity);
}
