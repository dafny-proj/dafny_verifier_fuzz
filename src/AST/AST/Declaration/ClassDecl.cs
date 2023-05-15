namespace AST;

public class ClassDecl
: TopLevelDecl, ConstructableFromDafny<Dafny.ClassDecl, ClassDecl> {
  public string? Name { get; }
  public List<MemberDecl> Members = new List<MemberDecl>();
  public bool IsDefaultClass => Name == null;

  public ClassDecl(string? name = null) {
    Name = name;
  }

  private ClassDecl(Dafny.ClassDecl cdd)
  : this(cdd.IsDefaultClass ? null : cdd.Name) {
    Members.AddRange(cdd.Members.Select(MemberDecl.FromDafny));
  }

  public static ClassDecl FromDafny(Dafny.ClassDecl dafnyNode) {
    return new ClassDecl(dafnyNode);
  }

  public void AddMember(MemberDecl md) {
    Members.Add(md);
  }

  public override IEnumerable<Node> Children => Members;
}