namespace AST_new;

public partial class ModuleDecl : TopLevelDecl {
  public readonly List<TopLevelDecl> Decls = new();

  public ModuleDecl(IEnumerable<TopLevelDecl>? ds = null) {
    if (ds != null) {
      Decls.AddRange(ds);
    }
  }

  public static ModuleDecl Skeleton() => new ModuleDecl();
  public void AddDecl(TopLevelDecl decl) => Decls.Add(decl);
  public void AddDecls(IEnumerable<TopLevelDecl> decls)
    => Decls.AddRange(decls);

  public override IEnumerable<Node> Children => Decls;
}