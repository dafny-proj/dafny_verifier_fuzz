namespace Fuzzer;

public class LoopUnrollMutationFinder : ASTVisitor {
  private List<LoopUnrollMutation> _Mutations = new List<LoopUnrollMutation>();
  private void AddMutation() {
    _Mutations.Add(new LoopUnrollMutation());
  }

  public IReadOnlyList<LoopUnrollMutation> Mutations => _Mutations.AsReadOnly();
  public int NumMutationsFound => Mutations.Count();

  public IReadOnlyList<LoopUnrollMutation> FindMutations(Program p) {
    VisitProgram(p);
    return Mutations;
  }

  private bool IsCandidateForLoopUnroll(Statement s) {
    return s is WhileStmt;
  }

  public override void VisitStmt(Statement s) {
    if (IsCandidateForLoopUnroll(s)) {
      AddMutation();
    }
    base.VisitStmt(s);
  }

}