namespace Fuzzer;

public class LoopRewriteMutationFinder : ASTVisitor {
  private List<LoopRewriteMutation> _Mutations = new List<LoopRewriteMutation>();
  private void AddMutation(Node loop) {
    _Mutations.Add(new LoopRewriteMutation(GetParent(), loop));
  }

  public IReadOnlyList<LoopRewriteMutation> Mutations => _Mutations.AsReadOnly();
  public int NumMutationsFound => Mutations.Count();

  public IReadOnlyList<LoopRewriteMutation> FindMutations(Program p) {
    VisitProgram(p);
    return Mutations;
  }

  private bool IsCandidateForLoopRewrite(Statement s) {
    return s is WhileStmt or ForLoopStmt;
  }

  public override void VisitStmt(Statement s) {
    if (IsCandidateForLoopRewrite(s)) {
      AddMutation(s);
    }
    base.VisitStmt(s);
  }

}