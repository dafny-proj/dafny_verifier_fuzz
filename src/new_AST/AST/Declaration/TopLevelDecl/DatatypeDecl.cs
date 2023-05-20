using System.Diagnostics.Contracts;

namespace AST_new;

public partial class DatatypeDecl : TopLevelDecl {
  public override string Name { get; protected set; }
  public readonly List<TypeParameter> TypeParams = new();
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
  public DatatypeConstructorDecl GetConstructor(string name)
    => Constructors.Find(c => c.Name == name)!;
  public DatatypeDiscriminatorDecl GetDiscriminator(string name)
    => Discriminators[name];
  public DatatypeDestructorDecl GetDestructor(string name)
    => Destructors[name];

  // Member declarations contain a reference to this enclosing declaration, so
  // before the declaration has been constructed, it shouldn't be possible to 
  // construct its members. Hence, member arguments are not included here.
  public DatatypeDecl(string name,
  IEnumerable<TypeParameter>? typeParams = null) {
    Name = name;
    if (typeParams != null) {
      TypeParams.AddRange(typeParams);
    }
  }

  public static DatatypeDecl Skeleton(string name,
  IEnumerable<TypeParameter>? typeParams = null)
    => new DatatypeDecl(name, typeParams);

  public void AddConstructor(DatatypeConstructorDecl constructor) {
    Contract.Requires(constructor.EnclosingDecl == this
      && !AllConstructorNames().Contains(constructor.Name));
    Constructors.Add(constructor);
    // Create discriminator for constructor.
    Discriminators.Add(
      constructor.Name, new DatatypeDiscriminatorDecl(constructor));
    // Create destructors, if not already existed, for constructor formals.
    var seenFormals = AllDestructorNames();
    foreach (var f in constructor.Formals) {
      if (seenFormals.Contains(f.Name)) {
        Contract.Requires(f.Type == Destructors[f.Name].Type);
        continue;
      }
      Destructors.Add(f.Name, new DatatypeDestructorDecl(this, f));
    }
  }

  public void AddMember(MemberDecl member) {
    Contract.Requires(member.EnclosingDecl == this
      && member is not (DatatypeDestructorDecl or DatatypeDiscriminatorDecl));
    if (member is DatatypeConstructorDecl constructor) {
      AddConstructor(constructor);
    } else {
      Members.Add(member);
    }
  }

}
