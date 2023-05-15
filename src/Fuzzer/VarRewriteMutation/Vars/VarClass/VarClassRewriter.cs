namespace Fuzzer;

public class VarClassRewriter : ASTVisitor {
  public Program Program { get; }
  // The node corresponding to the scope of the variables being merged.
  public Node Node { get; }
  private VarClass VC { get; }
  private VarClassRewriteTaskManager TM { get; }

  public VarClassRewriter(List<VarDecl> vars, Node node, Program program) {
    Program = program;
    Node = node;
    VC = new VarClass(vars);
    TM = new VarClassRewriteTaskManager(VC, new ParentMap(node));
  }

  public void Rewrite() {
    TM.AddClassDeclInsertTask(Program.DefaultModuleDef);
    if (Node is BlockStmt bs) {
      TM.AddInstanceDeclInsertTask(bs);
      VisitNode(Node);
    } else {
      throw new NotSupportedException($"Variable merging not supported for variables in `{Node.GetType()}`");
    }
    TM.CompleteTasks();
  }

  public override void VisitExpr(Expression e) {
    if (e is IdentifierExpr ie) {
      VisitIdentifierExpr(ie);
    } else {
      base.VisitExpr(e);
    }
  }

  public override void VisitStmt(Statement s) {
    if (s is VarDeclStmt vds) {
      VisitVarDeclStmt(vds);
    } else {
      base.VisitStmt(s);
    }
  }

  private void VisitIdentifierExpr(IdentifierExpr ie) {
    if (VC.ContainsVar(ie.Name)) {
      TM.AddIdentifierRewriteTask(ie);
    }
  }

  private void VisitVarDeclStmt(VarDeclStmt vds) {
    VisitChildren(vds);
    foreach (var vd in vds.Decls) {
      if (VC.ContainsVar(vd.Name)) {
        TM.AddVarDeclRewriteTask(vds);
        break;
      }
    }
  }
}