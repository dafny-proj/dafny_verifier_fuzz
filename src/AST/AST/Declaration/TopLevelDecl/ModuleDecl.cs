namespace AST;

public partial class ModuleDecl : TopLevelDecl {
  public readonly List<TopLevelDecl> Decls = new();

  public ModuleDecl(IEnumerable<TopLevelDecl>? ds = null) {
    if (ds != null) {
      Decls.AddRange(ds);
    }
  }

  public static ModuleDecl Skeleton() => new ModuleDecl();
  public void AppendDecl(TopLevelDecl decl) => Decls.Add(decl);
  public void InsertDecl(TopLevelDecl decl, int i) => Decls.Insert(i, decl);
  public void PrependDecl(TopLevelDecl decl) => Decls.Insert(0, decl);
  public void AddDecls(IEnumerable<TopLevelDecl> decls)
    => Decls.AddRange(decls);

  public override IEnumerable<Node> Children => Decls;
}