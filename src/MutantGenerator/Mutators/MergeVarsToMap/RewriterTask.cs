namespace MutantGenerator;

public partial class MergeVarsToMapMutationRewriter {
  private class VarRefRewriteTask : Task {
    private IdentifierExpr vref;
    private Node vrefParent;
    private MergeVarsToMapMutationRewriter vm;

    public VarRefRewriteTask(IdentifierExpr use, Node useParent,
    MergeVarsToMapMutationRewriter vm) {
      this.vref = use;
      this.vrefParent = useParent;
      this.vm = vm;
    }

    public override void Execute() {
      Contract.Assert(vref.Var is LocalVar);
      vrefParent.ReplaceChild(vref, vm.GenMapElementForVar((LocalVar)vref.Var));
    }
  }

  private class VarDefRewriteTask : Task {
    private AssignStmt vdef;
    private BlockStmt vdefParent;
    private MergeVarsToMapMutationRewriter vm;

    public VarDefRewriteTask(AssignStmt vdef, BlockStmt vdefParent,
    MergeVarsToMapMutationRewriter vm) {
      this.vdef = vdef;
      this.vdefParent = vdefParent;
      this.vm = vm;
    }

    public override void Execute() {
      var (mapAssignment, unaffectedVarAssignments) = vm.SplitAssignStmt(vdef);
      var replacement = new AssignStmt(unaffectedVarAssignments.Append(mapAssignment));
      vdefParent.Replace(vdef, replacement);
    }
  }

  private class VarDeclRewriteTask : Task {
    private VarDeclStmt vdec;
    private BlockStmt vdecParent;
    private MergeVarsToMapMutationRewriter vm;

    public VarDeclRewriteTask(VarDeclStmt vdec, BlockStmt vdecParent,
    MergeVarsToMapMutationRewriter vm) {
      this.vdec = vdec;
      this.vdecParent = vdecParent;
      this.vm = vm;
    }

    // Dafny does parallel assignment, which means any variables just declared on
    // the LHS cannot appear in any of the initialisers (RHS). This allows us to 
    // mutate the order of declarations in a statement without semantic changes.
    public override void Execute() {
      var replacement = new List<Statement>();
      var unaffectedVars = vdec.Vars.Where(v => !vm.ContainsVar(v));

      if (vdec.Initialiser == null) {
        // Simply remove the affected declarations if there is no initialiser.
        if (unaffectedVars.Count() > 0) {
          replacement.Add(new VarDeclStmt(unaffectedVars));
        }
        vdecParent.Replace(vdec, replacement);
        return;
      }

      var initialiser = vdec.Initialiser;
      if (initialiser is AssignStmt s) {
        // Move initialising expressions on affected declarations to an assignment.
        var (mapAssignment, unaffectedVarAssignments) = vm.SplitAssignStmt(s);
        if (unaffectedVars.Count() > 0) {
          replacement.Add(new VarDeclStmt(vars: unaffectedVars,
            initialiser: new AssignStmt(unaffectedVarAssignments)));
        }
        replacement.Add(new AssignStmt(mapAssignment));
        vdecParent.Replace(vdec, replacement);
        return;
      }

      throw new UnsupportedMutationException();
    }
  }

  private (AssignmentPair, List<AssignmentPair>) SplitAssignStmt(AssignStmt s) {
    var affectedVarAssignments = new List<ExpressionPair>();
    var unaffectedVarAssignments = new List<AssignmentPair>();
    foreach (var assignment in s.Assignments) {
      var lhs = assignment.Key;
      var rhs = assignment.Value;
      var lv = TryGetAffectedVar(lhs);
      if (lv != null) {
        if (rhs is not ExprRhs) {
          throw new UnsupportedMutationException();
        }
        rhs = (ExprRhs)rhs;
        affectedVarAssignments.Add(new ExpressionPair(
          key: GenMapIndexForVar(lv), value: ((ExprRhs)rhs).E));
      } else {
        unaffectedVarAssignments.Add(assignment);
      }
    }
    Expression newMap;
    if (affectedVarAssignments.Count() == 1) {
      newMap = GenMapUpdateValue(affectedVarAssignments[0]);
    } else {
      newMap = GenMapUpdateValues(affectedVarAssignments);
    }
    return (GenMapAssignment(newMap), unaffectedVarAssignments);
  }

}
