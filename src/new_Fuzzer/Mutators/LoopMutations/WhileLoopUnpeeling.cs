namespace Fuzzer_new;

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
public partial class WhileLoopUnpeelingRewriter {
  private WhileLoopStmt loop;
  private BlockStmt loopParent;
  private LocalVar? breakVariable;
  private string? breakLabel;
  private IGenerator gen;

  public WhileLoopUnpeelingRewriter(WhileLoopStmt loop, BlockStmt loopParent,
  IGenerator generator) {
    this.loop = loop;
    this.loopParent = loopParent;
    gen = generator;
  }

  private void Rewrite() {
    var unpeeledLoop = new List<Statement>();
    // Extract an iteration of the loop into an if statement.
    var guard = loop.Guard;
    var body = loop.Body!;
    var extractedIteration = new IfStmt(
      guard: guard == null ? null : Cloner.Clone<Expression>(guard),
      thn: Cloner.Clone<BlockStmt>(body));

    // Recondition break statements in the extracted iteration.
    new BreakReconditioner(this).Recondition(extractedIteration.Thn);

    if (breakVariable == null) {
      unpeeledLoop.AddRange(GenAssertFromInvariants());
      unpeeledLoop.Add(extractedIteration);
      unpeeledLoop.AddRange(GenAssertFromInvariants());
      unpeeledLoop.Add(loop);
    } else {
      // Create break variable declaration.
      var initBreakToFalse = new AssignStmt(new AssignmentPair(
        GenBreakVariableIdent(), new ExprRhs(new BoolLiteralExpr(false))));
      var breakDecl = new VarDeclStmt(breakVariable, initBreakToFalse);
      unpeeledLoop.Add(breakDecl);
      unpeeledLoop.AddRange(GenAssertFromInvariants());

      // Set break label for extracted iteration.
      extractedIteration.AddLabel(GetBreakLabel());
      unpeeledLoop.Add(extractedIteration);

      // Wrap assertions and remaining loop in a check for the break.
      var notBreak = new UnaryExpr(UnaryExpr.Opcode.Not, GenBreakVariableIdent());
      var remLoop = new IfStmt(guard: notBreak,
        thn: new BlockStmt(GenAssertFromInvariants().Append<Statement>(loop)));
      unpeeledLoop.Add(remLoop);
    }

    // Rewrite the loop in the parent.
    loopParent.Replace(loop, unpeeledLoop);
  }

  private IdentifierExpr GenBreakVariableIdent() {
    if (breakVariable == null) {
      breakVariable = new LocalVar(gen.GenVarName(), Type.Bool, Type.Bool);
    }
    return new IdentifierExpr(breakVariable);
  }

  private string GetBreakLabel() {
    if (breakLabel == null) {
      breakLabel = gen.GenLabelName();
    }
    return breakLabel;
  }

  private IEnumerable<AssertStmt> GenAssertFromInvariants() {
    return loop.Invariants?.GetUserDefinedSpec()
      .Select(e => new AssertStmt(Cloner.Clone<Expression>(e)))
      ?? Enumerable.Empty<AssertStmt>();
  }

  private class BreakReconditioner {
    private List<Task> reconditionTasks = new();
    private WhileLoopUnpeelingRewriter _wr;

    private int loopNestingDepth;
    private void EnterLoop() => loopNestingDepth++;
    private void ExitLoop() => loopNestingDepth--;
    private bool IsALoop(Node n) => n is LoopStmt;

    private Dictionary<Node, Node> childToParent = new();
    private Node GetParent(Node c) {
      if (!childToParent.ContainsKey(c)) {
        throw new ParentNotFoundException(c);
      }
      return childToParent[c];
    }
    private void SetParent(Node c, Node p) {
      if (childToParent.ContainsKey(c)) {
        throw new DuplicateParentException(c, childToParent[c], p);
      }
      childToParent.Add(c, p);
    }

    public BreakReconditioner(WhileLoopUnpeelingRewriter wr) {
      _wr = wr;
    }

    private void Reset() {
      reconditionTasks.Clear();
      childToParent.Clear();
      loopNestingDepth = 1;
    }

    public void Recondition(BlockStmt bodyToRecondition) {
      Reset();
      VisitNode(bodyToRecondition);
      reconditionTasks.ForEach(t => t.Execute());
    }

    private bool OfInterest(Node n) {
      switch (n) {
        case BlockStmt:
        case IfStmt:
        case LoopStmt:
        case BreakStmt:
          return true;
        default:
          return false;
      }
    }

    private void VisitNode(Node n) {
      if (IsALoop(n)) { EnterLoop(); }
      if (n is BreakStmt s) { VisitBreakStmt(s); }
      foreach (var c in n.Children) {
        if (OfInterest(c)) {
          SetParent(c, n);
          VisitNode(c);
        }
      }
      if (IsALoop(n)) { ExitLoop(); }
    }

    private void VisitBreakStmt(BreakStmt s) {
      if (s.HasTargetLabel()) { return; }
      var netBreak = s.Count - loopNestingDepth;
      if (netBreak > 0) {
        reconditionTasks.Add(new BreakOuterLoopReconditionTask(s));
      } else if (netBreak == 0) {
        Contract.Assert(GetParent(s) is BlockStmt);
        reconditionTasks.Add(
          new BreakThisLoopReconditionTask(s, (BlockStmt)GetParent(s), _wr));
      } else {
        // Doesn't break out of current body. No need to recondition.
      }
    }
  }

  private class BreakOuterLoopReconditionTask : Task {
    private BreakStmt _s;
    public BreakOuterLoopReconditionTask(BreakStmt s) {
      _s = s;
    }
    public override void Execute() {
      // Breaks above the current body. Since control flow will jump outside the 
      // current body, there is no need to track the current loop break.
      // It suffices to just adjust the number of breaks.
      Contract.Assert(_s.Count > 0);
      _s.Count--;
    }
  }

  private class BreakThisLoopReconditionTask : Task {
    private BreakStmt _s;
    private BlockStmt _parent;
    private WhileLoopUnpeelingRewriter _wr;
    public BreakThisLoopReconditionTask(BreakStmt s, BlockStmt parent,
    WhileLoopUnpeelingRewriter wr) {
      _s = s;
      _parent = parent;
      _wr = wr;
    }
    public override void Execute() {
      // Breaks exactly out of the current body. Record the break using a variable
      // to check if we should continue further iterations on the unpeeled loop.
      // Wrap the first iteration of the loop with a label to break out of.
      // `break;` => `breakVar := true; break breakLabel;`
      var setBreakVarToTrue = new AssignStmt(new AssignmentPair(
        _wr.GenBreakVariableIdent(), new ExprRhs(new BoolLiteralExpr(true))));
      var breakOutOfBody = new BreakStmt(_wr.GetBreakLabel());
      _parent.Replace(_s, new Statement[] { setBreakVarToTrue, breakOutOfBody });
    }
  }
}
