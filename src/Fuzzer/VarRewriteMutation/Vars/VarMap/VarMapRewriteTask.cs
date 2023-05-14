namespace Fuzzer;

public abstract class VarMapRewriteTask : Task {
  protected VarMap VM { get; set; }
  protected VarMapRewriteTask(VarMap vm) {
    VM = vm;
  }
}

public class VarMapIdentifierRewrite : VarMapRewriteTask {
  private IdentifierExpr Target { get; }
  private Node Parent { get; }

  public VarMapIdentifierRewrite(VarMap vm, IdentifierExpr target, Node parent)
  : base(vm) {
    Target = target;
    Parent = parent;
  }

  protected override void Action() {
    Parent.ReplaceChild(Target, VM.GenMapVarElement(Target.Name));
  }
}

public class VarMapVarDeclRewrite : VarMapRewriteTask {
  private VarDeclStmt Target { get; }
  private BlockStmt Parent { get; }

  public VarMapVarDeclRewrite(VarMap vm, VarDeclStmt target, BlockStmt parent)
  : base(vm) {
    Target = target;
    Parent = parent;
  }

  // Dafny does parallel assignment, which means any variables just declared on
  // the LHS cannot appear in any of the initialisers (RHS). This allows us to 
  // mutate the order of declarations in a statement without semantic changes.
  protected override void Action() {
    List<VarDecl> declsToRemove = new();
    List<AssignStmt.Assignment> assignmentsToGen = new();
    foreach (var vd in Target.Decls) {
      if (!VM.ContainsVar(vd.Name)) { continue; }
      declsToRemove.Add(vd);
      if (!vd.HasInitialiser()) { continue; }
      if (vd.Initialiser is AssignmentInitialiser ai) {
        if (ai.Value is ExprRhs er) {
          assignmentsToGen.Add(VM.GenMapVarAssignment(vd.Name, er.Expr));
        } else {
          throw new NotImplementedException();
        }
      } else {
        throw new NotImplementedException();
      }
    }
    Target.RemoveVarDecls(declsToRemove);
    List<Statement> replacement = new();
    if (!Target.IsEmpty()) {
      replacement.Add(Target);
    }
    if (assignmentsToGen.Count > 0) {
      replacement.Add(new AssignStmt(assignmentsToGen));
    }
    if (replacement.Count == 0) {
      Parent.RemoveChild(Target);
    } else {
      Parent.ReplaceChild(Target, new BlockStmt(replacement));
    }
  }
}

public class VarMapAssignRewrite : VarMapRewriteTask {
  private AssignStmt.Assignment Target { get; }
  private AssignStmt Parent { get; }

  public VarMapAssignRewrite(VarMap vm, AssignStmt.Assignment target, AssignStmt parent)
  : base(vm) {
    Target = target;
    Parent = parent;
  }

  protected override void Action() {
    if ((Target.Lhs is IdentifierExpr ie) && VM.ContainsVar(ie.Name)) {
      if (Target.Rhs is ExprRhs er) {
        var newAssignment = VM.GenMapVarAssignment(ie.Name, er.Expr);
        Parent.ReplaceChild(Target, newAssignment);
      } else {
        // e.g. TypeRhs values need to be assigned on a new statement. Map 
        // updates cannot contain an inline TypeRhs.
        throw new NotImplementedException();
      }
    } else {
      throw new NotImplementedException();
    }
  }
}
