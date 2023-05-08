namespace Fuzzer;

public class LoopUnroll {
  public class Writer : ILoopWriter {
    public bool CanWriteLoop(ALoop loop) {
      // TODO: Index-based loops.
      // TODO: Add condition for break to labels + continues.
      return loop is AConditionalLoop;
    }

    public Node WriteLoop(ALoop loop) {
      if (!CanWriteLoop(loop)) {
        throw new NotSupportedException($"LoopUnroll.Writer: Loop unrolling not handled.");
      }
      if (loop is AConditionalLoop cl) {
        return UnrollConditionalLoop(cl);
      }
      throw new NotImplementedException();
    }

    // TODO: Add invariant asserts
    /// <summary>
    /// `while cond { body }`
    /// Unrolls to
    /// ```
    /// if cond { body }
    /// while cond { body }
    /// ```
    /// </summary>
    private Node UnrollConditionalLoop(AConditionalLoop loop) {
      BlockStmt result = new BlockStmt();
      IfStmt firstIter = GenOneIterOfLoop(loop);
      Statement remIter = GenRemIterOfLoop(loop);

      // Recondition break statements in the out-of-loop iteration.
      var breakReconditioner = new BreakReconditioner();
      breakReconditioner.Recondition(firstIter.Thn);

      // Generate break variable declaration and label if used during reconditioning.
      var breakVar = breakReconditioner.BreakVar;
      if (breakVar != null) {
        result.Append(GenBreakVarDecl(breakVar));
        firstIter.Label = breakReconditioner.BreakLabel;
        // Check if we broke before continuing with the next iterations.
        remIter = new IfStmt(UnaryExpr.CreateNegExpr(breakVar.Clone()),
          new BlockStmt(new[] { remIter }));
      }

      // Merge to form unrolled loop.
      result.Append(firstIter);
      result.Append(remIter);
      return result;
    }

    // Generates an IfStmt corresponding to one iteration of the original loop.
    private IfStmt GenOneIterOfLoop(AConditionalLoop loop) {
      return new IfStmt(loop.Condition?.Clone(), loop.Body.Clone());
    }
    // Generates a WhileStmt corresponding to the loop after one unroll.
    // Should be identical to the original loop.
    private WhileStmt GenRemIterOfLoop(AConditionalLoop loop) {
      // TODO: Refactor usage of while loop writer.
      return (WhileStmt)new WhileLoop.Writer().WriteLoop(loop);
    }
    private VarDeclStmt GenBreakVarDecl(IdentifierExpr breakVI) {
      // TODO: Refactor general variable declaration utility.
      // `var breakVar: bool := false;`
      return new VarDeclStmt(
        new VarDecl(breakVI.Name, Type.Bool, new BoolLiteralExpr(false)));
    }
  }
}

public class BreakReconditioner : ASTVisitor {
  private int LoopNestingDepth = 1;
  private bool IsALoop(Node n) {
    return n is WhileStmt or ForLoopStmt;
  }
  private void EnterLoop() => LoopNestingDepth++;
  private void ExitLoop() => LoopNestingDepth--;
  private List<ReplaceChildTask> BreakReplaceTasks = new();
  public IdentifierExpr? BreakVar { get; private set; }
  public string? BreakLabel { get; private set; }
  private IdentifierExpr GetOrGenBreakVar() {
    // TODO: Check if name already exists in scope.
    BreakVar = BreakVar ?? new IdentifierExpr("breakVar", Type.Bool);
    return BreakVar;
  }
  private string GetOrGenBreakLabel() {
    // TODO: Check if name already exists in scope.
    BreakLabel = "breakLabel";
    return BreakLabel;
  }
  private UpdateStmt GenBreakVarSetToTrue() {
    return new UpdateStmt(GetOrGenBreakVar().Clone(),
      new ExprRhs(new BoolLiteralExpr(true)));
  }
  private BreakStmt GenBreakToLabel() {
    return new BreakStmt(GetOrGenBreakLabel());
  }

  public void Recondition(Statement loopBody) {
    VisitNode(loopBody);
    BreakReplaceTasks.ForEach(t => t.Execute());
  }

  public override void VisitStmt(Statement s) {
    var isALoop = IsALoop(s);
    if (isALoop) { EnterLoop(); }
    if (s is BreakStmt bs) { ReconditionBreakStmt(bs); }
    base.VisitStmt(s);
    if (isALoop) { ExitLoop(); }
  }

  private void ReconditionBreakStmt(BreakStmt bs) {
    var netBreak = bs.Count - LoopNestingDepth;
    // Doesn't break out of current body.
    if (netBreak < 0) return;
    // Breaks above the current body. Since control flow will jump outside the 
    // current body, there is no need to track the current loop break.
    // It suffices to just adjust the number of breaks.
    if (netBreak > 0) {
      bs.Count--;
      return;
    }
    // Breaks exactly out of the current body. Record the break using a variable
    // to check if we should continue further iterations on the unrolled loop.
    // Wrap the first iteration of the loop with a label to break out of.
    // `break;` => `breakVar := true; break breakLabel;`
    var bsReplacement = new BlockStmt(
      new Statement[] { GenBreakVarSetToTrue(), GenBreakToLabel() });
    // Don't replace immediately as it will interfere with AST traversal.
    BreakReplaceTasks.Add(new ReplaceChildTask(GetParent(), bs, bsReplacement));
  }

}
