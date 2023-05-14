namespace Fuzzer;

// Bottom up traversal is important here to ensure that the children are already 
// correct when mutating the parent.
public class VarMapRewriter : ASTVisitor {
  // The node corresponding to the scope of the variables being merged.
  public Node Node { get; }
  public VarMap VM { get; }
  public VarMapRewriteTaskManager TM { get; }

  public VarMapRewriter(List<VarDecl> vars, Node node) {
    Node = node;
    VM = new VarMap(vars);
    TM = new VarMapRewriteTaskManager(VM, new ParentMap(node));
  }

  public List<VarDecl> SelectVarsToMerge(Scope scope) {
    // TODO: Do type checking and random selection.
    return scope.Vars.Values.ToList();
  }

  public void Rewrite() {
    if (Node is BlockStmt bs) {
      bs.Prepend(VM.GenMapVarDecl());
      VisitNode(Node);
    } else {
      throw new NotSupportedException($"Variable merging not supported for variables in `{Node.GetType()}`");
    }
    TM.CompleteTasks();
  }

  public override void VisitExpr(Expression e) {
    switch (e) {
      case IdentifierExpr ie:
        VisitIdentifierExpr(ie);
        break;
      default:
        base.VisitExpr(e);
        break;
    }
  }

  public override void VisitStmt(Statement s) {
    switch (s) {
      case VarDeclStmt vds:
        VisitVarDeclStmt(vds);
        break;
      case AssignStmt ass:
        VisitAssignStmt(ass);
        break;
      default:
        base.VisitStmt(s);
        break;
    }
  }

  private void VisitIdentifierExpr(IdentifierExpr ie) {
    if (VM.ContainsVar(ie.Name)) {
      TM.AddIdentifierRewriteTask(ie);
    }
  }

  private void VisitVarDeclStmt(VarDeclStmt vds) {
    VisitChildren(vds);
    foreach (var vd in vds.Decls) {
      if (VM.ContainsVar(vd.Name)) {
        TM.AddVarDeclRewriteTask(vds);
        break;
      }
    }
  }

  private void VisitAssignStmt(AssignStmt ass) {
    bool needsRewrite = false;
    foreach (var a in ass.Assignments) {
      // Only visit the Rhs. Lhs identifiers are to be rewritten differently.
      VisitAssignRhs(a.Rhs);
      if ((a.Lhs is IdentifierExpr ie)) {
        if (VM.ContainsVar(ie.Name)) {
          needsRewrite = true;
        }
      } else {
        throw new NotImplementedException();
      }
    }
    if (needsRewrite) {
      TM.AddAssignStmtRewriteTask(ass);
    }
  }
}