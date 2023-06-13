namespace Fuzzer;

public partial class VarWithSideEffectingAssignmentFinder {
  public static HashSet<LocalVar> Find(Node n) {
    return new VarWithSideEffectingAssignmentFinder()._Find(n);
  }

  private HashSet<LocalVar> vars = new();
  private void AddVar(LocalVar v) { vars.Add(v); }
  private void TryAddVar(Expression e) {
    if (e is IdentifierExpr i && i.Var is LocalVar v) { AddVar(v); }
  }

  private HashSet<LocalVar> _Find(Node n) {
    VisitNode(n);
    return vars;
  }

  public void VisitNode(Node n) {
    switch (n) {
      case AssignStmt s:
        VisitAssignStmt(s);
        return;
      case CallStmt s:
        VisitCallStmt(s);
        return;
      case VarDeclStmt s:
        VisitVarDeclStmt(s);
        return;
      default:
        VisitChildren(n);
        return;
    }
  }

  private void VisitAssignStmt(AssignStmt s) {
    foreach (var a in s.Assignments) {
      if (a.Value is not ExprRhs) { TryAddVar(a.Key); }
    }
  }

  private void VisitCallStmt(CallStmt s) {
    foreach (var lhs in s.Lhss) { TryAddVar(lhs); }
  }

  private void VisitVarDeclStmt(VarDeclStmt s) {
    if (s.Initialiser == null) { return; }
    if (s.Initialiser is AssignStmt ss) {
      VisitAssignStmt(ss);
    } else if (s.Initialiser is CallStmt) {
      foreach (var v in s.Vars) { AddVar(v); }
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
      case Type:
      case Expression:
      case AssignmentRhs:
        return false;
      default:
        return true;
    }
  }

}