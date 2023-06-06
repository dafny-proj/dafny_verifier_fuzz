namespace AST;

public partial class ClassDecl : TopLevelDecl { }
public partial class DefaultClassDecl : ClassDecl { }
public partial class ArrayClassDecl : ClassDecl { }
public partial class ArrowTypeDecl : ClassDecl { }

public partial class ClassDecl : TopLevelDecl {
  public override string Name { get; protected set; }
  public readonly List<TypeParameterDecl> TypeParams = new();
  public readonly List<MemberDecl> Members = new();

  // Member declarations contain a reference to this enclosing declaration, so
  // before the declaration has been constructed, it shouldn't be possible to 
  // construct its members. Hence, member arguments are not included here.
  public ClassDecl(string name, IEnumerable<TypeParameterDecl>? typeParams = null) {
    Name = name;
    if (typeParams != null) {
      TypeParams.AddRange(typeParams);
    }
  }

  public static ClassDecl Skeleton(string name,
  IEnumerable<TypeParameterDecl>? typeParams = null)
    => new ClassDecl(name, typeParams);
  public void AddMember(MemberDecl member) => Members.Add(member);
  public void AddMembers(IEnumerable<MemberDecl> members)
    => Members.AddRange(members);

  public override IEnumerable<Node> Children => TypeParams.Concat<Node>(Members);
}

public partial class DefaultClassDecl : ClassDecl {
  public DefaultClassDecl() : base(name: "_DF_default_class") { }

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

  public FieldDecl LengthField(int dim = 1) {
    var name = "Length" + (Dimensions == 1 ? "" : dim);
    var ln = Members.Find(m => m.Name == name);
    if (ln == null) {
      ln = new FieldDecl(this, name, Type.Int, isBuiltIn: true);
      AddMember(ln);
    }
    return (FieldDecl)ln;
  }
}

// TODO: This class inherits from ClassDecl which reflects the implementation in 
// Dafny. Consider removing the inheritance, they don't really correspond.
// Built-in arrow type class. (f: A ~> B). 
// This class gives rise to two subset types (f: A --> B) and (f: A -> B).
public partial class ArrowTypeDecl : ClassDecl {
  // Number of arguments to the function. Excludes the single produced result.
  public int Arity { get; }

  public ArrowTypeDecl(int arity) : base(name: "_DF_builtin_arrow") { }

  public static ArrowTypeDecl Skeleton(int arity) => new ArrowTypeDecl(arity);
}
