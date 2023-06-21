namespace MutantGenerator;

public partial class LocalVarTrackingScope {
  // Variables declared in this scope, not inclusive of variables declared in 
  // parent/child scopes.
  public List<LocalVar> Vars = new();
  // The block statement corresponding to this scope.
  public BlockStmt Node { get; }
  public ModuleDecl EnclosingModule { get; }
  public LocalVarTrackingScope? Parent { get; }
  public List<LocalVarTrackingScope> Children = new();

  public void AddVar(LocalVar var) => Vars.Add(var);
  public void RemoveVar(LocalVar var) => Vars.Remove(var);
  public void AddChild(LocalVarTrackingScope child) => Children.Add(child);

  public LocalVarTrackingScope(BlockStmt node, ModuleDecl enclosingModule,
  LocalVarTrackingScope? parent = null) {
    Node = node;
    EnclosingModule = enclosingModule;
    Parent = parent;
    Parent?.AddChild(this);
  }

}

public partial class LocalVarTrackingScopeBuilder {
  public static List<LocalVarTrackingScope> FindScopes(Node n) {
    return new LocalVarTrackingScopeBuilder()._FindScopes(n);
  }

  private Stack<ModuleDecl> modules = new();
  private LocalVarTrackingScope? curScope;
  private List<LocalVarTrackingScope> allScopes = new();
  private void Reset() {
    modules.Clear();
    curScope = null;
    allScopes.Clear();
  }
  private ModuleDecl CurModule() => modules.Peek();
  private void EnterModule(ModuleDecl d) => modules.Push(d);
  private void ExitModule(ModuleDecl d) => modules.Pop();
  private void EnterScope(BlockStmt s) {
    var newScope = new LocalVarTrackingScope(s, CurModule(), curScope);
    allScopes.Add(newScope);
    curScope = newScope;
  }
  private void ExitScope(BlockStmt s) {
    Contract.Assert(curScope != null);
    curScope = curScope!.Parent;
  }

  private List<LocalVarTrackingScope> _FindScopes(Node n) {
    Reset();
    VisitNode(n);
    return allScopes;
  }

  private void VisitNode(Node n) {
    switch (n) {
      case ModuleDecl d:
        VisitModuleDecl(d);
        return;
      case BlockStmt s:
        VisitBlockStmt(s);
        return;
      case LocalVar v:
        VisitLocalVar(v);
        return;
      default:
        VisitChildren(n);
        return;
    }
  }

  private void VisitModuleDecl(ModuleDecl d) {
    EnterModule(d);
    VisitChildren(d);
    ExitModule(d);
  }

  private void VisitBlockStmt(BlockStmt s) {
    EnterScope(s);
    VisitChildren(s);
    ExitScope(s);
  }

  private void VisitLocalVar(LocalVar v) {
    Contract.Assert(curScope != null);
    curScope!.AddVar(v);
  }

  private void VisitChildren(Node n) {
    foreach (var c in n.Children) {
      if (OfInterest(c)) {
        VisitNode(c);
      }
    }
  }

  private bool OfInterest(Node n) {
    switch (n) {
      case Expression:
      case AssignmentRhs:
        return false;
      default:
        return true;
    }
  }
}