namespace MutantGenerator;

// Variables of the same scope and of known type, which are not modified in a 
// nested loop can be merged to a class.
public partial class MergeVarsToClassMutationFinder {
  public static List<MergeVarsToClassMutation> FindMutations(Node n) {
    var finder = new MergeVarsToClassMutationFinder();
    finder.VisitNode(n);
    var mutations = new List<MergeVarsToClassMutation>();
    foreach (var s in finder.allScopes) {
      if (s.Vars.Count == 0) { continue; }
      mutations.Add(new MergeVarsToClassMutation(
        vars: s.Vars,
        enclosingScope: s.Node,
        enclosingModule: s.EnclosingModule
      ));
    }
    return mutations;
  }

  private MergeVarsToClassMutationFinder() { }

  private Stack<ModuleDecl> modules = new();
  private Stack<LoopStmt> loops = new();
  private LocalVarTrackingScope? curScope;
  private List<LocalVarTrackingScope> allScopes = new();
  private Dictionary<LoopStmt, HashSet<LocalVar>> varsDeclInLoop = new();
  private Dictionary<LocalVar, LocalVarTrackingScope> varToDeclScope = new();

  private ModuleDecl CurModule() => modules.Peek();
  private void EnterModule(ModuleDecl d) => modules.Push(d);
  private void ExitModule(ModuleDecl d) => modules.Pop();
  private bool InLoop() => loops.Count > 0;
  private HashSet<LocalVar> GetVarsDeclaredInCurrentLoop()
    => varsDeclInLoop[loops.Peek()];
  private void EnterLoop(LoopStmt s) {
    loops.Push(s);
    varsDeclInLoop.Add(s, new());
  }
  private void ExitLoop(LoopStmt s) => loops.Pop();
  private void EnterScope(BlockStmt s) {
    var newScope = new LocalVarTrackingScope(s, CurModule(), curScope);
    allScopes.Add(newScope);
    curScope = newScope;
  }
  private void ExitScope(BlockStmt s) {
    Contract.Assert(curScope != null);
    curScope = curScope!.Parent;
  }
  private bool IsVarTracked(LocalVar v) => varToDeclScope.ContainsKey(v);
  private void TrackVar(LocalVar v) {
    Contract.Assert(curScope != null);
    curScope!.AddVar(v);
    varToDeclScope.Add(v, curScope!);
    if (InLoop()) { GetVarsDeclaredInCurrentLoop().Add(v); }
  }
  private void UntrackVar(LocalVar v) {
    varToDeclScope[v].RemoveVar(v);
    varToDeclScope.Remove(v);
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
      case LoopStmt s:
        VisitLoopStmt(s);
        return;
      case UpdateStmt s:
        VisitUpdateStmt(s);
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

  private void VisitLoopStmt(LoopStmt s) {
    EnterLoop(s);
    VisitChildren(s);
    ExitLoop(s);
  }

  private void VisitLocalVar(LocalVar v) {
    // Only consider variables with known types for this mutation.
    if (v.Type is TypeProxy) { return; }
    TrackVar(v);
  }

  private void VisitUpdateStmt(UpdateStmt s) {
    // Variable modified in a loop but are declared outside of the loop are not
    // considered for this mutation due to heap semantics.
    if (!InLoop()) { return; }
    var varsDeclaredWithinThisLoop = GetVarsDeclaredInCurrentLoop();
    foreach (var lhs in s.Lhss) {
      if (lhs is IdentifierExpr e && e.Var is LocalVar v) {
        if (IsVarTracked(v) && !varsDeclaredWithinThisLoop.Contains(v)) {
          UntrackVar(v);
        }
      }
    }
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