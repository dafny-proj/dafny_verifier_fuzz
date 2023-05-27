namespace Fuzzer;

// Requires loops to have bodies.
// Requires loop bodies to not have labeled statements.
/// <summary>
/// `while cond { body }`
/// Unpeels to
/// ```
/// if cond { body }
/// while cond { body }
/// ```
/// </summary>
public class WhileLoopUnpeelMutation: IMutation {
  public WhileLoopStmt whileLoop;
  public BlockStmt enclosingScope;

  public
  WhileLoopUnpeelMutation(WhileLoopStmt whileLoop, BlockStmt enclosingScope) {
    this.whileLoop = whileLoop;
    this.enclosingScope = enclosingScope;
  }
}

public class WhileLoopUnpeelMutator : BasicMutator<WhileLoopUnpeelMutation> {
  public IGenerator Gen { get; }
  public WhileLoopUnpeelMutator(Randomizer rand, IGenerator gen) : base(rand) {
    Gen = gen;
  }

  public override List<WhileLoopUnpeelMutation> FindPotentialMutations(Program p) {
    return new WhileLoopUnpeelMutationFinder().FindMutations(p);
  }

  public override WhileLoopUnpeelMutation
  SelectMutation(List<WhileLoopUnpeelMutation> ms) {
    Contract.Requires(ms.Count > 0);
    return Rand.RandElement(ms);
  }

  public override void ApplyMutation(WhileLoopUnpeelMutation m) {
    new WhileLoopUnpeelMutationRewriter(m, Gen).Rewrite();
  }
}

public class WhileLoopUnpeelMutationFinder {
  public List<WhileLoopUnpeelMutation> FindMutations(Node n) {
    Reset();
    VisitNode(n);
    return mutations;
  }

  private List<WhileLoopUnpeelMutation> mutations = new();
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
      mutations.Add(new WhileLoopUnpeelMutation(
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
