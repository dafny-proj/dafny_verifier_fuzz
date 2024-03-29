using System.Diagnostics.Contracts;

namespace AST;

public partial class DatatypeDecl : TopLevelDecl { }
public partial class TupleTypeDecl : DatatypeDecl { }

public partial class DatatypeDecl : TopLevelDecl {
  public override string Name { get; protected set; }
  public readonly List<TypeParameterDecl> TypeParams = new();
  public readonly List<DatatypeConstructorDecl> Constructors = new();
  // Auto-generated.
  public readonly Dictionary<string, DatatypeDestructorDecl> Destructors = new();
  // Auto-generated.
  public readonly Dictionary<string, DatatypeDiscriminatorDecl> Discriminators = new();
  // User-defined (i.e. not auto-generated), non-constructor members.
  public readonly List<MemberDecl> Members = new();

  public HashSet<string> AllConstructorNames()
    => Constructors.Select(c => c.Name).ToHashSet();
  public HashSet<string> AllDestructorNames()
    => Destructors.Keys.ToHashSet();
  public bool HasConstructor(string name)
    => Constructors.Exists(c => c.Name == name);
  public DatatypeConstructorDecl GetConstructor(string name)
    => Constructors.Find(c => c.Name == name)!;
  public DatatypeDiscriminatorDecl GetDiscriminator(string name)
    => Discriminators[name];
  public DatatypeDestructorDecl GetDestructor(string name)
    => Destructors[name];
  public bool HasMembers() => Members.Count > 0;

  // Member declarations contain a reference to this enclosing declaration, so
  // before the declaration has been constructed, it shouldn't be possible to 
  // construct its members. Hence, member arguments are not included here.
  public DatatypeDecl(string name,
  IEnumerable<TypeParameterDecl>? typeParams = null) {
    Name = name;
    if (typeParams != null) {
      TypeParams.AddRange(typeParams);
    }
  }

  public static DatatypeDecl Skeleton(string name,
  IEnumerable<TypeParameterDecl>? typeParams = null)
    => new DatatypeDecl(name, typeParams);

  public void PrependConstructor(DatatypeConstructorDecl constructor) {
    InsertConstructor(constructor, index: 0);
  }
  public void AppendConstructor(DatatypeConstructorDecl constructor) {
    InsertConstructor(constructor, index: Constructors.Count);
  }
  public void InsertConstructor(DatatypeConstructorDecl constructor, int index) {
    Contract.Requires(constructor.EnclosingDecl == this
      && !AllConstructorNames().Contains(constructor.Name));
    Constructors.Insert(index, constructor);
    // Create discriminator for constructor.
    var discriminator = new DatatypeDiscriminatorDecl(constructor);
    Discriminators.Add(discriminator.Name, discriminator);
    // Create destructors, if not already existed, for constructor formals.
    var seenFormals = AllDestructorNames();
    foreach (var f in constructor.Parameters) {
      if (!seenFormals.Contains(f.Name)) {
        Destructors.Add(f.Name, new DatatypeDestructorDecl(this, f));
      }
      Contract.Assert(f.Type == Destructors[f.Name].Type);
      Destructors[f.Name].AddConstructor(constructor);
    }
  }

  public void AddConstructors(IEnumerable<DatatypeConstructorDecl> constructors) {
    foreach (var c in constructors) {
      AppendConstructor(c);
    }
  }

  public void AddMember(MemberDecl member) {
    Contract.Requires(member.EnclosingDecl == this
      && member is not (DatatypeDestructorDecl or DatatypeDiscriminatorDecl));
    if (member is DatatypeConstructorDecl constructor) {
      AppendConstructor(constructor);
    } else {
      Members.Add(member);
    }
  }

  public void AddMembers(IEnumerable<MemberDecl> members) {
    foreach (var m in members) {
      AddMember(m);
    }
  }

  public override IEnumerable<Node> Children
    => TypeParams.Concat<Node>(Constructors).Concat<Node>(Members);

}

public partial class TupleTypeDecl : DatatypeDecl {
  public TupleTypeDecl(string name,
  IEnumerable<TypeParameterDecl>? typeParams = null)
  : base(name, typeParams) { }

  public new static TupleTypeDecl Skeleton(string name,
  IEnumerable<TypeParameterDecl>? typeParams = null)
    => new TupleTypeDecl(name, typeParams);

  // TODO: Auto-generate a tuple type and its accessors. 
  // public TupleTypeDecl(int dims)
}
