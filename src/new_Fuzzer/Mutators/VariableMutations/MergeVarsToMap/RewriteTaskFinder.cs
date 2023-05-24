namespace Fuzzer_new;

public partial class MergeVarsToMapRewriter {
  private Dictionary<Node, Node> childToParent = new();
  private Node GetParent(Node c) {
    if (!childToParent.ContainsKey(c)) {
      throw new ParentNotFoundException(c);
    }
    return childToParent[c];
  }
  private void SetParent(Node c, Node p) {
    if (childToParent.ContainsKey(c)) {
      throw new DuplicateParentException(c, childToParent[c], p);
    }
    childToParent.Add(c, p);
  }

  private void VisitNode(Node n) {
    switch (n) {
      case IdentifierExpr e:
        VisitIdentifierExpr(e);
        return;
      case VarDeclStmt s:
        VisitVarDeclStmt(s);
        return;
      case AssignStmt s:
        VisitAssignStmt(s);
        return;
      default:
        VisitChildren(n);
        return;
    }
  }

  private void VisitIdentifierExpr(IdentifierExpr e) {
    if (e.Var is LocalVar v && ContainsVar(v)) {
      rewriteTasks.Add(new VarRefRewriteTask(e, GetParent(e), this));
    }
  }

  private void VisitVarDeclStmt(VarDeclStmt s) {
    VisitChildren(s);
    if (s.Vars.Any(v => ContainsVar(v))) {
      Contract.Assert(GetParent(s) is BlockStmt);
      rewriteTasks.Add(new VarDeclRewriteTask(s, (BlockStmt)GetParent(s), this));
    }
  }

  private void VisitAssignStmt(AssignStmt s) {
    // Don't visit the Lhs.
    VisitChildren(s, s.Rhss);
    if (s.Lhss.Any(v => TryGetAffectedVar(v) != null)) {
      Contract.Assert(GetParent(s) is BlockStmt);
      rewriteTasks.Add(new VarDefRewriteTask(s, (BlockStmt)GetParent(s), this));
    }
  }

  private void VisitChildren(Node parent, IEnumerable<Node>? children = null) {
    var cs = children ?? parent.Children;
    foreach (var c in cs) {
      if (OfInterest(c)) {
        SetParent(c, parent);
        VisitNode(c);
      }
    }
  }

  private bool OfInterest(Node n) {
    switch (n) {
      case Type:
        return false;
      default:
        return true;
    }
  }

}