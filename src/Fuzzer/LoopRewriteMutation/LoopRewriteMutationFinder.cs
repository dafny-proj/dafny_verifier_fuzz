namespace Fuzzer;

public class LoopRewriteMutationFinder : ASTVisitor {
  private List<LoopRewriteMutation> _Mutations = new List<LoopRewriteMutation>();
  private void AddMutation(Node loop, Node parent) {
    _Mutations.Add(new LoopRewriteMutation(parent, loop));
  }
  private ParentMap PM;

  public IReadOnlyList<LoopRewriteMutation> Mutations => _Mutations.AsReadOnly();
  public int NumMutationsFound => Mutations.Count();

  public LoopRewriteMutationFinder(ParentMap pm) {
    PM = pm;
  }

  public IReadOnlyList<LoopRewriteMutation> FindMutations(Program p) {
    VisitProgram(p);
    return Mutations;
  }

  private bool IsCandidateForLoopRewrite(Statement s) {
    // Ignore uninteresting (?) cases where the loop body is empty.
    if (s is WhileStmt ws) {
      return ws.Body != null;
    }
    if (s is ForLoopStmt fs) {
      return fs.Body != null;
    }
    return false;
  }

  public override void VisitStmt(Statement s) {
    if (IsCandidateForLoopRewrite(s)) {
      AddMutation(s, PM.GetParent(s));
    }
    base.VisitStmt(s);
  }

}