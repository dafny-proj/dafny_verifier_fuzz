namespace Fuzzer;

public partial class WhileLoopPeelMutationRewriter {
  private WhileLoopStmt whileLoop;
  private BlockStmt enclosingScope;
  private LocalVar? breakVariable;
  private string? breakLabel;
  private IGenerator gen;

  public WhileLoopPeelMutationRewriter(WhileLoopPeelMutation m, IGenerator g)
  : this(m.whileLoop, m.enclosingScope, g) { }

  public WhileLoopPeelMutationRewriter(
  WhileLoopStmt whileLoop, BlockStmt enclosingScope, IGenerator generator) {
    this.whileLoop = whileLoop;
    this.enclosingScope = enclosingScope;
    this.gen = generator;
  }

  public void Rewrite() {
    var peeledLoop = new List<Statement>();
    // Extract an iteration of the loop into an if statement.
    var guard = whileLoop.Guard;
    var body = whileLoop.Body!;
    var extractedIteration = new IfStmt(
      guard: guard == null ? null : Cloner.Clone<Expression>(guard),
      thn: Cloner.Clone<BlockStmt>(body));

    // Recondition break statements in the extracted iteration.
    new BreakReconditioner(this).Recondition(extractedIteration.Thn);

    if (breakVariable != null) {
      // Create break variable declaration.
      var initBreakToFalse = new AssignStmt(new AssignmentPair(
        GenBreakVariableIdent(), new ExprRhs(new BoolLiteralExpr(false))));
      var breakDecl = new VarDeclStmt(breakVariable, initBreakToFalse);
      peeledLoop.Add(breakDecl);
    }

    peeledLoop.AddRange(GenAssertFromInvariants());

    if (breakLabel != null) {
      // Set break label for extracted iteration.
      extractedIteration.AddLabel(GetBreakLabel());
    }
    peeledLoop.Add(extractedIteration);

    if (breakVariable != null) {
      // Wrap remaining loop in a check for the break.
      var notBreak = new UnaryExpr(UnaryExpr.Opcode.Not, GenBreakVariableIdent());
      var remLoop = new IfStmt(guard: notBreak,
        thn: new BlockStmt(new[] { whileLoop }));
      peeledLoop.Add(remLoop);
    } else {
      peeledLoop.Add(whileLoop);
    }

    // Rewrite the loop in the parent.
    enclosingScope.Replace(whileLoop, peeledLoop);
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
    return whileLoop.Invariants?.GetUserDefinedSpec()
      .Select(e => new AssertStmt(Cloner.Clone<Expression>(e)))
      ?? Enumerable.Empty<AssertStmt>();
  }

  private class BreakReconditioner {
    private List<Task> reconditionTasks = new();
    private WhileLoopPeelMutationRewriter wr;

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

    public BreakReconditioner(WhileLoopPeelMutationRewriter wr) {
      this.wr = wr;
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
          new BreakThisLoopReconditionTask(s, (BlockStmt)GetParent(s), wr));
      } else {
        // Doesn't break out of current body. No need to recondition.
      }
    }
  }

  private class BreakOuterLoopReconditionTask : Task {
    private BreakStmt s;
    public BreakOuterLoopReconditionTask(BreakStmt s) {
      this.s = s;
    }
    public override void Execute() {
      // Breaks above the current body. Since control flow will jump outside the 
      // current body, there is no need to track the current loop break.
      // It suffices to just adjust the number of breaks.
      Contract.Assert(s.Count > 0);
      s.Count--;
    }
  }

  private class BreakThisLoopReconditionTask : Task {
    private BreakStmt s;
    private BlockStmt parent;
    private WhileLoopPeelMutationRewriter wr;
    public BreakThisLoopReconditionTask(BreakStmt s, BlockStmt parent,
    WhileLoopPeelMutationRewriter wr) {
      this.s = s;
      this.parent = parent;
      this.wr = wr;
    }
    public override void Execute() {
      // Breaks exactly out of the current body.
      var replacement = new List<Statement>();
      if (s is not ContinueStmt) {
        // If we have a `break`, record using a variable to ensure we don't try
        // to continue further iteraions of the peeled loop. Otherwise, we can 
        // `continue` with the loop.
        var setBreakVarToTrue = new AssignStmt(new AssignmentPair(
          wr.GenBreakVariableIdent(), new ExprRhs(new BoolLiteralExpr(true))));
        replacement.Add(setBreakVarToTrue);
      }
      // Wrap the first iteration of the loop with a label to break out of.
      var breakOutOfBody = new BreakStmt(wr.GetBreakLabel());
      replacement.Add(breakOutOfBody);
      parent.Replace(s, replacement);
    }
  }
}
