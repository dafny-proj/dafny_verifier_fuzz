using System.Diagnostics.Contracts;

namespace Fuzzer;

public class VarMap {
  string MapName { get; set; }
  MapType MapType { get; set; }
  List<string> Vars = new();

  public VarMap(List<VarDecl> vars) {
    var valueT = GetCommonType(vars);
    if (valueT == null) {
      throw new ArgumentException(
        $"Only variables of the same type can be merged into a map.");
    }
    MapName = GenMapName();
    // The keys are the namestrings of the variables.
    // The common type of the variables forms the value type.
    MapType = new MapType(kt: Type.String, vt: valueT);
    Vars.AddRange(vars.Select(v => v.Name));
  }

  public Type? GetCommonType(List<VarDecl> vars) {
    if (vars.Count() > 0) {
      var type = vars[0].Type;
      var common = vars.All(v => type == v.Type);
      return common ? type : null;
    }
    return null;
  }

  public bool ContainsVar(string name) {
    return Vars.Contains(name);
  }

  // `var m: map<string, valueT> := map[]`
  public VarDeclStmt GenMapVarDecl() {
    MapDisplayExpr emptyMap = new MapDisplayExpr(MapType);
    VarDecl mapDecl = new VarDecl(MapName, MapType, init: emptyMap);
    return new VarDeclStmt(mapDecl);
  }

  // TODO: Generate multiple identifiers or have a singleton identifier?
  public IdentifierExpr GenMapVarIdent() {
    return new IdentifierExpr(MapName, MapType);
  }

  // `a` -> `m["a"]`
  public StringLiteralExpr GenMapVarIndex(string var) {
    Contract.Requires(this.ContainsVar(var));
    return new StringLiteralExpr(var);
  }

  // `a := 1` -> `m := m["a" := 1]`
  public AssignStmt.Assignment GenMapVarAssignment(string var, Expression value) {
    return new AssignStmt.Assignment(GenMapVarIdent(), new ExprRhs(
      new CollectionUpdateExpr(GenMapVarIdent(), GenMapVarIndex(var), value)));
  }

  // TODO: Handle random, non-colliding name generation.
  private string GenMapName() {
    return "map_merge";
  }
}

public class VarMapRewriteTaskManager : TaskManager {
  private VarMap VM { get; }
  private ParentMap PM { get; }
  private Dictionary<Node, List<Task>> NodeTasks = new();

  public VarMapRewriteTaskManager(VarMap vm, ParentMap pm) {
    VM = vm;
    PM = pm;
  }

  public void AddIdentifierRewriteTask(IdentifierExpr ie) {
    var task = new VarMapIdentifierRewrite(VM, ie, PM.GetParent(ie));
    AddTaskForNode(ie, task);
  }

  public void AddVarDeclRewriteTask(VarDeclStmt vds) {
    var parent = PM.GetParent(vds);
    Contract.Assert(parent is BlockStmt);
    var task = new VarMapVarDeclRewrite(VM, vds, (parent as BlockStmt)!);
    AddTaskForNode(vds, task);
  }

  public void AddAssignmentRewriteTask(AssignStmt.Assignment a) {
    var parent = PM.GetParent(a);
    Contract.Assert(parent is AssignStmt);
    var task = new VarMapAssignRewrite(VM, a, (parent as AssignStmt)!);
    AddTaskForNode(a, task);
  }

  private void AddTaskForNode(Node n, Task t) {
    // Add child tasks as dependencies.
    foreach (var c in n.Children) {
      if (NodeTasks.ContainsKey(c)) {
        // TODO: This is not accurate as it only checks direct children.
        t.AddBlockingDependency(NodeTasks[c]);
      }
    }
    // Record the task belonging to the node.
    NodeTasks.GetValueOrDefault(n, new()).Add(t);
    AddTask(t);
  }
}

public class VarMapIdentifierRewrite : Task {
  private VarMap VM { get; }
  private IdentifierExpr Target { get; }
  private Node Parent { get; }

  public VarMapIdentifierRewrite(VarMap vm, IdentifierExpr target, Node parent) {
    VM = vm;
    Target = target;
    Parent = parent;
  }

  protected override void Action() {
    Parent.ReplaceChild(Target, VM.GenMapVarIndex(Target.Name));
  }
}

public class VarMapVarDeclRewrite : Task {
  private VarMap VM { get; }
  private VarDeclStmt Target { get; }
  private BlockStmt Parent { get; }

  public VarMapVarDeclRewrite(VarMap vm, VarDeclStmt target, BlockStmt parent) {
    VM = vm;
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

public class VarMapAssignRewrite : Task {
  private VarMap VM { get; }
  private AssignStmt.Assignment Target { get; }
  private AssignStmt Parent { get; }

  public VarMapAssignRewrite(VarMap vm, AssignStmt.Assignment target, AssignStmt parent) {
    VM = vm;
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

// TODO: Check that parent tracking is correct here.
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

  // `a` -> `m["a"]`
  private void VisitIdentifierExpr(IdentifierExpr ie) {
    if (VM.ContainsVar(ie.Name)) {
      TM.AddIdentifierRewriteTask(ie);
    }
  }

  // `var a := 1` -> `m := m["a" := 1]`
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
    ass.Assignments.ForEach(VisitAssignment);
  }

  // `a := 1` -> `m := m["a" := 1]`
  private void VisitAssignment(AssignStmt.Assignment a) {
    // TODO: parent tracking is lost here.
    // Only visit the Rhs. Lhs identifiers are to be rewritten differently.
    VisitAssignRhs(a.Rhs);

    if ((a.Lhs is IdentifierExpr ie)) {
      if (VM.ContainsVar(ie.Name)) {
        TM.AddAssignmentRewriteTask(a);
      }
    } else {
      throw new NotImplementedException();
    }
  }
}