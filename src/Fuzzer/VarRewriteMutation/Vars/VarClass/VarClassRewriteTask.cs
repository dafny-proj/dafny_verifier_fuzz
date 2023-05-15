using System.Diagnostics.Contracts;

namespace Fuzzer;

public class VarClassRewriteTaskManager : TaskManager {
  private VarClass VC { get; }
  private ParentMap PM { get; }

  public VarClassRewriteTaskManager(VarClass vc, ParentMap pm) {
    VC = vc;
    PM = pm;
  }

  public void AddClassDeclInsertTask(ModuleDefinition m) {
    AddTask(new VarClassClassDeclInsert(VC, m));
  }

  public void AddInstanceDeclInsertTask(BlockStmt s) {
    AddTask(new VarClassInstanceDeclInsert(VC, s));
  }

  public void AddIdentifierRewriteTask(IdentifierExpr ie) {
    AddTask(new VarClassIdentifierRewrite(VC, ie, PM.GetParent(ie)));
  }

  public void AddVarDeclRewriteTask(VarDeclStmt vds) {
    var parent = PM.GetParent(vds);
    Contract.Assert(parent is BlockStmt);
    AddTask(new VarClassVarDeclRewrite(VC, vds, (parent as BlockStmt)!));
  }
}

public abstract class VarClassRewriteTask : Task {
  protected VarClass VC { get; }
  protected VarClassRewriteTask(VarClass vc) {
    VC = vc;
  }
}

public class VarClassClassDeclInsert : VarClassRewriteTask {
  public ModuleDefinition EnclosingModule { get; }

  public VarClassClassDeclInsert(VarClass vc, ModuleDefinition em) : base(vc) {
    EnclosingModule = em;
  }

  protected override void Action() {
    EnclosingModule.PrependDecl(VC.GenClassDecl());
  }
}

public class VarClassInstanceDeclInsert : VarClassRewriteTask {
  public BlockStmt EnclosingScope { get; }

  public VarClassInstanceDeclInsert(VarClass vc, BlockStmt bs) : base(vc) {
    EnclosingScope = bs;
  }

  protected override void Action() {
    EnclosingScope.Prepend(VC.GenInstanceDecl());
  }
}

public class VarClassIdentifierRewrite : VarClassRewriteTask {
  private IdentifierExpr Target { get; }
  private Node Parent { get; }

  public VarClassIdentifierRewrite(VarClass vc, IdentifierExpr target, Node parent) : base(vc) {
    Target = target;
    Parent = parent;
  }

  protected override void Action() {
    Parent.ReplaceChild(Target, VC.GenField(Target.Name));
  }
}

public class VarClassVarDeclRewrite : VarClassRewriteTask {
  private VarDeclStmt Target { get; }
  private BlockStmt Parent { get; }

  public VarClassVarDeclRewrite(VarClass vc, VarDeclStmt target, BlockStmt parent) : base(vc) {
    Target = target;
    Parent = parent;
  }

  protected override void Action() {
    List<VarDecl> declsToRemove = new();
    List<VarDecl> declsToConvert = new();
    foreach (var vd in Target.Decls) {
      if (VC.ContainsVar(vd.Name)) {
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
      List<AssignStmt.Assignment> assignments = new();
      foreach (var d in declsToConvert) {
        Contract.Assert(d.HasInitialiser() && d.Initialiser is AssignmentInitialiser);
        var initialiser = (d.Initialiser as AssignmentInitialiser)!;
        assignments.Add(new AssignStmt.Assignment(
          VC.GenField(d.Name), initialiser.Value));
      }
      replacement.Add(new AssignStmt(assignments));
    }

    if (replacement.Count == 0) {
      Parent.RemoveChild(Target);
    } else {
      Parent.ReplaceChild(Target, new BlockStmt(replacement));
    }
  }
}
