namespace Fuzzer;

// Requires loops to have bodies.
/// <summary>
/// `for i := s to e { body }`
/// Rewrites to
/// ```
/// var i := s;
/// while (i != e) { body; i := i + 1; }
/// ```
/// </summary>
public class ForLoopToWhileLoopMutation : IMutation {
  public ForLoopStmt forLoop;
  public BlockStmt enclosingScope;

  public
  ForLoopToWhileLoopMutation(ForLoopStmt forLoop, BlockStmt enclosingScope) {
    this.forLoop = forLoop;
    this.enclosingScope = enclosingScope;
  }
}

public class ForLoopToWhileLoopMutator
: BasicMutator<ForLoopToWhileLoopMutation> {
  public ForLoopToWhileLoopMutator(Randomizer rand) : base(rand) { }

  public override List<ForLoopToWhileLoopMutation>
  FindPotentialMutations(Program p) {
    return new ForLoopToWhileLoopMutationFinder().FindMutations(p);
  }

  public override ForLoopToWhileLoopMutation
  SelectMutation(List<ForLoopToWhileLoopMutation> ms) {
    Contract.Requires(ms.Count > 0);
    return Rand.RandElement(ms);
  }

  public override void ApplyMutation(ForLoopToWhileLoopMutation m) {
    new ForLoopToWhileLoopMutationRewriter(m).Rewrite();
  }
}

public class ForLoopToWhileLoopMutationFinder {
  public List<ForLoopToWhileLoopMutation> FindMutations(Node n) {
    Reset();
    VisitNode(n);
    return mutations;
  }

  private List<ForLoopToWhileLoopMutation> mutations = new();
  private Stack<BlockStmt> scopes = new();
  private void EnterScope(BlockStmt s) => scopes.Push(s);
  private void ExitScope(BlockStmt s) => scopes.Pop();
  private BlockStmt GetEnclosingScope() => scopes.Peek();
  private void Reset() {
    mutations.Clear();
    scopes.Clear();
  }

  public void VisitNode(Node n) {
    switch (n) {
      case BlockStmt s:
        VisitBlockStmt(s);
        return;
      case ForLoopStmt s:
        VisitForLoopStmt(s);
        return;
      default:
        VisitChildren(n);
        return;
    }
  }

  private void VisitBlockStmt(BlockStmt s) {
    EnterScope(s);
    VisitChildren(s);
    ExitScope(s);
  }

  private void VisitForLoopStmt(ForLoopStmt s) {
    if (s.HasBody()) {
      mutations.Add(new ForLoopToWhileLoopMutation(
        forLoop: s, enclosingScope: GetEnclosingScope()));
    }
    VisitChildren(s);
  }

  private void VisitChildren(Node n) {
    foreach (var c in n.Children) {
      if (OfInterest(c)) {
        VisitNode(c);
      }
    }
  }

  private bool OfInterest(Node n) {
    switch (n) {
      case Program:
      case Declaration:
      case Statement:
        return true;
      default:
        return false;
    }
  }

}
