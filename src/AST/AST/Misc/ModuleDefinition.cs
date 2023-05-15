namespace AST;

public class ModuleDefinition
: Node, ConstructableFromDafny<Dafny.ModuleDefinition, ModuleDefinition> {
  public override IEnumerable<Node> Children => TopLevelDecls;

  public List<TopLevelDecl> TopLevelDecls = new List<TopLevelDecl>();

  private ModuleDefinition(Dafny.ModuleDefinition moduleDefDafny) {
    TopLevelDecls
      = moduleDefDafny.TopLevelDecls.Select(TopLevelDecl.FromDafny).ToList();
  }

  public static ModuleDefinition FromDafny(Dafny.ModuleDefinition dafnyNode) {
    return new ModuleDefinition(dafnyNode);
  }

  public void AppendDecl(TopLevelDecl tld) {
    TopLevelDecls.Add(tld);
  }

  public void PrependDecl(TopLevelDecl tld) {
    TopLevelDecls.Insert(0, tld);
  }
}