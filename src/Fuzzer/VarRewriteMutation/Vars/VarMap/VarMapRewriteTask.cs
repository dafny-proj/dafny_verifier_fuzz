using System.Diagnostics.Contracts;

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
    List<VarDecl> declsToConvert = new();
    foreach (var vd in Target.Decls) {
      if (VM.ContainsVar(vd.Name)) {
        declsToRemove.Add(vd);
        if (vd.HasInitialiser()) {
          declsToConvert.Add(vd);
        }
      }
    }

    List<Statement> replacement = new();
    Target.RemoveVarDecls(declsToRemove);
    if (!Target.IsEmpty()) {
      replacement.Add(Target);
    }

    if (declsToConvert.Count > 0) {
      Expression rhs;
      if (declsToConvert.Count == 1) {
        rhs = SingleDeclToMapUpdate(declsToConvert[0]);
      } else {
        rhs = new BinaryExpr(BinaryExpr.Opcode.Add,
          VM.GenMapVarIdent(), MultipleDeclsToMap(declsToConvert));
      }
      replacement.Add(new AssignStmt(VM.GenMapVarIdent(), new ExprRhs(rhs)));
    }

    if (replacement.Count == 0) {
      Parent.RemoveChild(Target);
    } else {
      Parent.ReplaceChild(Target, new BlockStmt(replacement));
    }
  }

  private Expression ExtractInitialiser(VarDeclInitialiser init) {
    if (init is AssignmentInitialiser ai) {
      if (ai.Value is ExprRhs er) {
        return er.Expr;
      }
    }
    throw new NotImplementedException();
  }

  private CollectionUpdateExpr
  SingleDeclToMapUpdate(VarDecl vd) {
    Contract.Assert(vd.Initialiser != null);
    return VM.GenMapVarUpdate(vd.Name, ExtractInitialiser(vd.Initialiser!));
  }

  private MapDisplayExpr
  MultipleDeclsToMap(List<VarDecl> vds) {
    List<MapDisplayExprElement> elements = new();
    foreach (var vd in vds) {
      Contract.Assert(vd.Initialiser != null);
      var lhs = VM.GenMapVarIndex(vd.Name);
      elements.Add(new MapDisplayExprElement(lhs, ExtractInitialiser(vd.Initialiser!)));
    }
    return new MapDisplayExpr(VM.MapType, elements);
  }
}

public class VarMapAssignStmtRewrite : VarMapRewriteTask {
  private AssignStmt Target { get; }
  private BlockStmt Parent { get; }

  public VarMapAssignStmtRewrite(VarMap vm, AssignStmt target, BlockStmt parent)
  : base(vm) {
    Target = target;
    Parent = parent;
  }

  private bool NeedsRewrite(AssignStmt.Assignment a) {
    return (a.Lhs is IdentifierExpr ie) && VM.ContainsVar(ie.Name);
  }

  protected override void Action() {
    // Get all assignments that need to be rewritten.
    var assignmentsToRewrite = Target.Assignments.Where(NeedsRewrite).ToList();
    Expression newRhs;
    if (assignmentsToRewrite.Count == 1) {
      newRhs = SingleAssignmentToMapUpdate(assignmentsToRewrite[0]);
    } else {
      newRhs = new BinaryExpr(BinaryExpr.Opcode.Add,
        VM.GenMapVarIdent(),
        MultipleAssignmentsToMap(assignmentsToRewrite));
    }
    Target.RemoveAssignments(assignmentsToRewrite);
    Target.AddAssignment(VM.GenMapVarAssignment(newRhs));
  }

  private Expression ExtractRhs(AssignmentRhs rhs) {
    if (rhs is ExprRhs er) {
      return er.Expr;
    }
    // e.g. TypeRhs values need to be assigned on a new statement. Map 
    // updates cannot contain an inline TypeRhs.
    throw new NotImplementedException();
  }

  private CollectionUpdateExpr
  SingleAssignmentToMapUpdate(AssignStmt.Assignment assignment) {
    return VM.GenMapVarUpdate(assignment.Lhs, ExtractRhs(assignment.Rhs));
  }

  private MapDisplayExpr
  MultipleAssignmentsToMap(List<AssignStmt.Assignment> assignments) {
    List<MapDisplayExprElement> elements = new();
    foreach (var a in assignments) {
      Contract.Assert(a.Lhs is IdentifierExpr);
      var lhs = VM.GenMapVarIndex(a.Lhs);
      elements.Add(new MapDisplayExprElement(lhs, ExtractRhs(a.Rhs)));
    }
    return new MapDisplayExpr(VM.MapType, elements);
  }
}
