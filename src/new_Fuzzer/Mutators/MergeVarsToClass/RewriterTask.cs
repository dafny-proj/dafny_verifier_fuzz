namespace Fuzzer_new;

public partial class MergeVarsToClassMutationRewriter {
  private class VarRefRewriteTask : Task {
    private IdentifierExpr vref;
    private Node vrefParent;
    private MergeVarsToClassMutationRewriter vc;

    public VarRefRewriteTask(IdentifierExpr use, Node useParent,
    MergeVarsToClassMutationRewriter vc) {
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
    private MergeVarsToClassMutationRewriter vc;

    public VarDeclRewriteTask(VarDeclStmt vdec, BlockStmt vdecParent,
    MergeVarsToClassMutationRewriter vc) {
      this.vdec = vdec;
      this.vdecParent = vdecParent;
      this.vc = vc;
    }

    public override void Execute() {
      var replacement = new List<Statement>();
      var unaffectedVars = vdec.Vars.Where(v => !vc.ContainsVar(v));

      if (vdec.Initialiser == null) {
        // Simply remove the affected declarations if there is no initialiser.
        if (unaffectedVars.Count() > 0) {
          replacement.Add(new VarDeclStmt(unaffectedVars));
        }
        vdecParent.Replace(vdec, replacement);
        return;
      }

      var initialiser = vdec.Initialiser;
      if (initialiser is CallStmt c) {
        if (unaffectedVars.Count() > 0) {
          replacement.Add(new VarDeclStmt(unaffectedVars));
        }
        var lhss = new List<Expression>();
        foreach (var lhs in c.Lhss) {
          var lv = vc.TryGetAffectedVar(lhs);
          if (lv == null) {
            lhss.Add(lhs);
          } else {
            lhss.Add(vc.GenFieldRefOfVar(lv));
          }
        }
        replacement.Add(new CallStmt(call: c.Call, lhss: lhss));
        vdecParent.Replace(vdec, replacement);
        return;
      }

      if (initialiser is AssignStmt s) {
        var affectedVarAssignments = new List<AssignmentPair>();
        var unaffectedVarAssignments = new List<AssignmentPair>();
        foreach (var a in s.Assignments) {
          var lv = vc.TryGetAffectedVar(a.Key);
          if (lv == null) {
            unaffectedVarAssignments.Add(a);
          } else {
            affectedVarAssignments.Add(new AssignmentPair(
              key: vc.GenFieldRefOfVar(lv), value: a.Value));
          }
        }
        if (unaffectedVars.Count() > 0) {
          replacement.Add(new VarDeclStmt(vars: unaffectedVars,
            initialiser: new AssignStmt(unaffectedVarAssignments)));
        }
        replacement.Add(new AssignStmt(affectedVarAssignments));
        vdecParent.Replace(vdec, replacement);
        return;
      }

      throw new UnsupportedMutationException();
    }
  }
}
