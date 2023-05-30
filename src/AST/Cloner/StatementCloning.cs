namespace AST.Cloner;

public partial class ASTCloner {
  private void SetStatementLabel(Statement s, Statement c) {
    if (s.Labels != null && c.Labels == null) {
      c.Labels = s.Labels.Select(s => s).ToList();
    }
  }

  private Statement CloneStatement(Statement s) {
    return s switch {
      BlockStmt bs => CloneBlockStmt(bs),
      VarDeclStmt vs => CloneVarDeclStmt(vs),
      UpdateStmt us => CloneUpdateStmt(us),
      PrintStmt ps => ClonePrintStmt(ps),
      ReturnStmt rs => CloneReturnStmt(rs),
      IfStmt ifs => CloneIfStmt(ifs),
      WhileLoopStmt ws => CloneWhileLoopStmt(ws),
      ForLoopStmt fs => CloneForLoopStmt(fs),
      BreakStmt bs => CloneBreakStmt(bs),
      AssertStmt ass => CloneAssertStmt(ass),
      ForallStmt fs => CloneForallStmt(fs),
      MatchStmt ms => CloneMatchStmt(ms),
      _ => throw new UnsupportedNodeCloningException(s),
    };
  }

  private BlockStmt CloneBlockStmt(BlockStmt s) {
    var c = new BlockStmt(s.Body.Select(CloneStatement));
    SetStatementLabel(s, c);
    return c;
  }

  private VarDeclStmt CloneVarDeclStmt(VarDeclStmt s) {
    var c = new VarDeclStmt(s.Vars.Select(CloneLocalVar),
      s.Initialiser == null ? null : CloneUpdateStmt(s.Initialiser));
    SetStatementLabel(s, c);
    return c;
  }

  private UpdateStmt CloneUpdateStmt(UpdateStmt s) {
    return s switch {
      AssignStmt a => CloneUpdateStmt(a),
      CallStmt c => CloneUpdateStmt(c),
      _ => throw new UnsupportedNodeCloningException(s),
    };
  }

  private AssignStmt CloneUpdateStmt(AssignStmt s) {
    var c = new AssignStmt(s.Assignments.Select(CloneAssignmentPair));
    SetStatementLabel(s, c);
    return c;
  }

  private CallStmt CloneUpdateStmt(CallStmt s) {
    var c = new CallStmt(
      CloneMethodCallRhs(s.Call), s.Lhss.Select(CloneExpression));
    SetStatementLabel(s, c);
    return c;
  }

  private PrintStmt ClonePrintStmt(PrintStmt s) {
    var c = new PrintStmt(s.Expressions.Select(CloneExpression));
    SetStatementLabel(s, c);
    return c;
  }

  private ReturnStmt CloneReturnStmt(ReturnStmt s) {
    var c = new ReturnStmt(
      s.Returns == null ? null : CloneUpdateStmt(s.Returns));
    SetStatementLabel(s, c);
    return c;
  }

  private IfStmt CloneIfStmt(IfStmt s) {
    var c = new IfStmt(s.Guard == null ? null : CloneExpression(s.Guard),
      CloneBlockStmt(s.Thn), s.Els == null ? null : CloneStatement(s.Els));
    SetStatementLabel(s, c);
    return c;
  }

  private WhileLoopStmt CloneWhileLoopStmt(WhileLoopStmt s) {
    var c = new WhileLoopStmt(
      guard: s.Guard == null ? null : CloneExpression(s.Guard),
      body: s.Body == null ? null : CloneBlockStmt(s.Body),
      inv: CloneSpecification(s.Invariants),
      mod: CloneSpecification(s.Modifies),
      dec: CloneSpecification(s.Decreases));
    SetStatementLabel(s, c);
    return c;
  }

  private ForLoopStmt CloneForLoopStmt(ForLoopStmt s) {
    var c = new ForLoopStmt(
      loopIndex: CloneBoundVar(s.LoopIndex),
      goesUp: s.GoesUp,
      loopStart: CloneExpression(s.LoopStart),
      loopEnd: s.LoopEnd == null ? null : CloneExpression(s.LoopEnd),
      body: s.Body == null ? null : CloneBlockStmt(s.Body),
      inv: CloneSpecification(s.Invariants),
      mod: CloneSpecification(s.Modifies),
      dec: CloneSpecification(s.Decreases));
    SetStatementLabel(s, c);
    return c;
  }

  private BreakStmt CloneBreakStmt(BreakStmt s) {
    var count = s.Count;
    var target = s.TargetLabel;
    BreakStmt c;
    if (s is ContinueStmt) {
      c = target != null ? new ContinueStmt(target) : new ContinueStmt(count);
    } else {
      c = target != null ? new BreakStmt(target) : new BreakStmt(count);
    }
    SetStatementLabel(s, c);
    return c;
  }

  private AssertStmt CloneAssertStmt(AssertStmt s) {
    var c = new AssertStmt(CloneExpression(s.Assertion));
    SetStatementLabel(s, c);
    return c;
  }

  private ForallStmt CloneForallStmt(ForallStmt s) {
    return new ForallStmt(CloneQuantifierDomain(s.QuantifierDomain),
      CloneSpecification(s.Ensures),
      s.Body == null ? null : CloneStatement(s.Body));
  }

  private MatchStmt CloneMatchStmt(MatchStmt s) {
    return new MatchStmt(
      CloneExpression(s.Selector), s.Cases.Select(CloneMatchStmtCase));
  }

}
