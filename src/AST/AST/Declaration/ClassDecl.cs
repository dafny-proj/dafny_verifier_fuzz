namespace AST;

public class ClassDecl
: TopLevelDecl, ConstructableFromDafny<Dafny.ClassDecl, ClassDecl> {
  public override IEnumerable<Node> Children => Members;
  public List<MemberDecl> Members = new List<MemberDecl>();
  public readonly bool IsDefaultClass = false;

  private ClassDecl(Dafny.ClassDecl classDeclDafny) {
    Members.AddRange(classDeclDafny.Members.Select(MemberDecl.FromDafny));
    IsDefaultClass = classDeclDafny.IsDefaultClass;
  }

  public static ClassDecl FromDafny(Dafny.ClassDecl dafnyNode) {
    return new ClassDecl(dafnyNode);
  }
}