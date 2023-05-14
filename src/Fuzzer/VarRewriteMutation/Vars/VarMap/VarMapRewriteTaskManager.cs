using System.Diagnostics.Contracts;

namespace Fuzzer;

public class VarMapRewriteTaskManager : TaskManager {
  private VarMap VM { get; }
  private ParentMap PM { get; }

  public VarMapRewriteTaskManager(VarMap vm, ParentMap pm) {
    VM = vm;
    PM = pm;
  }

  public void AddIdentifierRewriteTask(IdentifierExpr ie) {
    var task = new VarMapIdentifierRewrite(VM, ie, PM.GetParent(ie));
    AddTask(task);
  }

  public void AddVarDeclRewriteTask(VarDeclStmt vds) {
    var parent = PM.GetParent(vds);
    Contract.Assert(parent is BlockStmt);
    var task = new VarMapVarDeclRewrite(VM, vds, (parent as BlockStmt)!);
    AddTask(task);
  }

  public void AddAssignStmtRewriteTask(AssignStmt a) {
    var parent = PM.GetParent(a);
    Contract.Assert(parent is BlockStmt);
    var task = new VarMapAssignStmtRewrite(VM, a, (parent as BlockStmt)!);
    AddTask(task);
  }

}