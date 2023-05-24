namespace Fuzzer_new;

public partial class MergeVarsToClassRewriter {
  private class VarRefRewriteTask : Task {
    private IdentifierExpr vref;
    private Node vrefParent;
    private MergeVarsToClassRewriter vc;

    public VarRefRewriteTask(IdentifierExpr use, Node useParent,
    MergeVarsToClassRewriter vc) {
      this.vref = use;
      this.vrefParent = useParent;
      this.vc = vc;
    }

    public override void Execute() {
      Contract.Assert(vref.Var is LocalVar);
      vrefParent.ReplaceChild(vref, vc.GenFieldRefOfVar((LocalVar)vref.Var));
    }
  }

  private class VarDeclRewriteTask : Task {
    private VarDeclStmt vdec;
    private BlockStmt vdecParent;
    private MergeVarsToClassRewriter vc;

    public VarDeclRewriteTask(VarDeclStmt vdec, BlockStmt vdecParent,
    MergeVarsToClassRewriter vc) {
      this.vdec = vdec;
      this.vdecParent = vdecParent;
      this.vc = vc;
    }

    public override void Execute() {
      var unaffectedVars = vdec.Vars.Where(v => !vc.ContainsVar(v));
      if (unaffectedVars.Count() == 0) {
        // Remove the entire statement if all declarations are affected.
        vdecParent.Remove(vdec);
        return;
      }
      var unaffectedVarsDecl = new VarDeclStmt(unaffectedVars);
      if (!vdec.HasInitialiser()) {
        // Simply remove the affected declarations if there is no initialiser.
        vdecParent.Replace(vdec, unaffectedVarsDecl);
        return;
      }
      // The initialiser is an AssignStmt if there are multiple declarations.
      var assignments = ((AssignStmt)vdec.Initialiser!).Assignments;
      var affected = vdec.Vars.Select(v => vc.ContainsVar(v));
      var affectedVarsAssignment = new AssignStmt();
      var unaffectedVarsAssignment = new AssignStmt();
      for (int i = 0; i < affected.Count(); i++) {
        if (affected.ElementAt(i)) {
          affectedVarsAssignment.AddAssignment(new AssignmentPair(
            key: vc.GenFieldRefOfVar((LocalVar)(vdec.Vars[i])),
            value: assignments[i].Value));
        } else {
          unaffectedVarsAssignment.AddAssignment(assignments[i]);
        }
      }
      unaffectedVarsDecl.Initialiser = unaffectedVarsAssignment;
      vdecParent.Replace(vdec,
        new Statement[] { unaffectedVarsDecl, affectedVarsAssignment });
    }
  }
}
