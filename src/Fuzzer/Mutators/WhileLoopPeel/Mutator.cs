namespace Fuzzer;

// Requires loops to have bodies.
// Requires loop bodies to not have labeled statements.
/// <summary>
/// `while cond { body }`
/// Peels to
/// ```
/// if cond { body }
/// while cond { body }
/// ```
/// </summary>
public class WhileLoopPeelMutation: IMutation {
  public WhileLoopStmt whileLoop;
  public BlockStmt enclosingScope;

  public
  WhileLoopPeelMutation(WhileLoopStmt whileLoop, BlockStmt enclosingScope) {
    this.whileLoop = whileLoop;
    this.enclosingScope = enclosingScope;
  }
}

public class WhileLoopPeelMutator : BasicMutator<WhileLoopPeelMutation> {
  public IGenerator Gen { get; }
  public WhileLoopPeelMutator(Randomizer rand, IGenerator gen) : base(rand) {
    Gen = gen;
  }

  public override List<WhileLoopPeelMutation> FindPotentialMutations(Program p) {
    return new WhileLoopPeelMutationFinder().FindMutations(p);
  }

  public override WhileLoopPeelMutation
  SelectMutation(List<WhileLoopPeelMutation> ms) {
    Contract.Requires(ms.Count > 0);
    return Rand.RandElement(ms);
  }

  public override void ApplyMutation(WhileLoopPeelMutation m) {
    new WhileLoopPeelMutationRewriter(m, Gen).Rewrite();
  }
}

public class WhileLoopPeelMutationFinder {
  public List<WhileLoopPeelMutation> FindMutations(Node n) {
    Reset();
    VisitNode(n);
    return mutations;
  }

  private List<WhileLoopPeelMutation> mutations = new();
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
      case WhileLoopStmt s:
        VisitWhileLoopStmt(s);
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

  private void VisitWhileLoopStmt(WhileLoopStmt s) {
    if (s.HasBody()) {
      mutations.Add(new WhileLoopPeelMutation(
      whileLoop: s, enclosingScope: GetEnclosingScope()));
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
