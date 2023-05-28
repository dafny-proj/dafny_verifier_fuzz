using Dafny = Microsoft.Dafny;

namespace AST.Translation;

public partial class ASTTranslator {
  private void SetStatementLabel(Dafny.Statement ds, Statement s) {
    if (ds.Labels != null && s.Labels == null) {
      s.Labels = new();
      var l = ds.Labels;
      while (l != null) {
        if (l.Data.Name != null) {
          s.Labels.Add(l.Data.Name);
        }
        l = l.Next;
      }
    }
  }
  private Statement TranslateStatement(Dafny.Statement ds) {
    Statement s = ds switch {
      Dafny.BlockStmt bs => TranslateBlockStmt(bs),
      Dafny.VarDeclStmt vds => TranslateVarDeclStmt(vds),
      Dafny.UpdateStmt us => TranslateUpdateStmt(us),
      Dafny.AssignStmt ass => TranslateAssignStmt(ass),
      Dafny.CallStmt cs => TranslateCallStmt(cs),
      Dafny.PrintStmt ps => TranslatePrintStmt(ps),
      Dafny.ReturnStmt rs => TranslateReturnStmt(rs),
      Dafny.IfStmt ifs => TranslateIfStmt(ifs),
      Dafny.LoopStmt ls => ls switch {
        Dafny.WhileStmt ws => TranslateWhileLoopStmt(ws),
        Dafny.ForLoopStmt fs => TranslateForLoopStmt(fs),
        _ => throw new UnsupportedTranslationException(ls)
      },
      Dafny.BreakStmt bs => TranslateBreakStmt(bs),
      Dafny.AssertStmt ats => TranslateAssertStmt(ats),
      _ => throw new UnsupportedTranslationException(ds),
    };
    SetStatementLabel(ds, s);
    return s;
  }

  private BlockStmt TranslateBlockStmt(Dafny.BlockStmt ds) {
    var s = new BlockStmt(ds.Body.Select(TranslateStatement));
    SetStatementLabel(ds, s);
    return s;
  }

  private PrintStmt TranslatePrintStmt(Dafny.PrintStmt ds) {
    var s = new PrintStmt(ds.Args.Select(TranslateExpression));
    SetStatementLabel(ds, s);
    return s;
  }

  private ReturnStmt TranslateReturnStmt(Dafny.ReturnStmt ds) {
    var s = new ReturnStmt(
      ds.HiddenUpdate == null ? null : TranslateUpdateStmt(ds.HiddenUpdate));
    SetStatementLabel(ds, s);
    return s;
  }

  private IfStmt TranslateIfStmt(Dafny.IfStmt ds) {
    var s = new IfStmt(
      guard: ds.Guard == null ? null : TranslateExpression(ds.Guard),
      thn: TranslateBlockStmt(ds.Thn),
      els: ds.Els == null ? null : TranslateStatement(ds.Els));
    SetStatementLabel(ds, s);
    return s;
  }

  private WhileLoopStmt TranslateWhileLoopStmt(Dafny.WhileStmt ds) {
    var s = new WhileLoopStmt(
      guard: ds.Guard == null ? null : TranslateExpression(ds.Guard),
      body: ds.Body == null ? null : TranslateBlockStmt(ds.Body),
      inv: TranslateSpecification(Specification.Type.Invariant, ds.Invariants),
      mod: TranslateSpecification(Specification.Type.ModifiesFrame, ds.Mod),
      dec: TranslateSpecification(Specification.Type.Decreases, ds.Decreases));
    SetStatementLabel(ds, s);
    return s;
  }

  private ForLoopStmt TranslateForLoopStmt(Dafny.ForLoopStmt ds) {
    var s = new ForLoopStmt(
      loopIndex: TranslateBoundVar(ds.LoopIndex),
      goesUp: ds.GoingUp,
      loopStart: TranslateExpression(ds.Start),
      loopEnd: ds.End == null ? null : TranslateExpression(ds.End),
      body: ds.Body == null ? null : TranslateBlockStmt(ds.Body),
      inv: TranslateSpecification(Specification.Type.Invariant, ds.Invariants),
      mod: TranslateSpecification(Specification.Type.ModifiesFrame, ds.Mod),
      dec: TranslateSpecification(Specification.Type.Decreases, ds.Decreases));
    SetStatementLabel(ds, s);
    return s;
  }

  private BreakStmt TranslateBreakStmt(Dafny.BreakStmt ds) {
    var count = ds.BreakAndContinueCount;
    var target = ds.TargetLabel == null ? null : ds.TargetLabel.val;
    BreakStmt s;
    if (ds.IsContinue) {
      s = target != null ? new ContinueStmt(target) : new ContinueStmt(count);
    } else {
      s = target != null ? new BreakStmt(target) : new BreakStmt(count);
    }
    SetStatementLabel(ds, s);
    return s;
  }

  private AssertStmt TranslateAssertStmt(Dafny.AssertStmt ds) {
    var s = new AssertStmt(TranslateExpression(ds.Expr));
    SetStatementLabel(ds, s);
    return s;
  }

  private VarDeclStmt TranslateVarDeclStmt(Dafny.VarDeclStmt ds) {
    var s = new VarDeclStmt(
      vars: ds.Locals.Select(TranslateLocalVar),
      initialiser: ds.Update == null ? null :
      (UpdateStmt)TranslateStatement(ds.Update));
    SetStatementLabel(ds, s);
    return s;
  }

  private UpdateStmt TranslateUpdateStmt(Dafny.UpdateStmt ds) {
    if (ds.ResolvedStatements == null) {
      throw new InvalidASTOperationException(
        $"Translation requires update statement to be resolved.");
    }
    UpdateStmt s;
    if (ds.ResolvedStatements.All(s => s is Dafny.AssignStmt)) {
      s = TranslateAssignStmt(
        ds.ResolvedStatements.Select(s => (Dafny.AssignStmt)s));
    } else if (ds.ResolvedStatements.Count() == 1
      && ds.ResolvedStatements[0] is Dafny.CallStmt) {
      s = TranslateCallStmt((Dafny.CallStmt)ds.ResolvedStatements[0]);
    } else {
      throw new UnsupportedTranslationException(ds);
    }
    SetStatementLabel(ds, s);
    return s;
  }

  private UpdateStmt TranslateCallStmt(Dafny.CallStmt ds) {
    var s = new CallStmt(call: new MethodCallRhs(
      callee: TranslateMemberSelectExpr(ds.MethodSelect),
      arguments: ds.Args.Select(TranslateExpression)),
      lhss: ds.Lhs.Select(TranslateExpression));
    SetStatementLabel(ds, s);
    return s;
  }

  private AssignStmt TranslateAssignStmt(Dafny.AssignStmt ds) {
    var s = TranslateAssignStmt(new[] { ds });
    SetStatementLabel(ds, s);
    return s;
  }

  private AssignStmt TranslateAssignStmt(IEnumerable<Dafny.AssignStmt> ds) {
    return new AssignStmt(ds.Select(a => new AssignmentPair(
      TranslateExpression(a.Lhs), TranslateAssignmentRhs(a.Rhs))));
  }
}
