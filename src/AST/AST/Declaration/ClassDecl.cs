namespace AST;

public class ClassDecl
: TopLevelDecl, ConstructableFromDafny<Dafny.ClassDecl, ClassDecl> {
  public string? Name { get; }
  public List<MemberDecl> Members = new List<MemberDecl>();
  public bool IsDefaultClass => Name == null;

  private ClassDecl(Dafny.ClassDecl cdd) {
    Members.AddRange(cdd.Members.Select(MemberDecl.FromDafny));
    Name = cdd.IsDefaultClass ? null : cdd.Name;
  }

  public static ClassDecl FromDafny(Dafny.ClassDecl dafnyNode) {
    return new ClassDecl(dafnyNode);
  }

  public override IEnumerable<Node> Children => Members;
}