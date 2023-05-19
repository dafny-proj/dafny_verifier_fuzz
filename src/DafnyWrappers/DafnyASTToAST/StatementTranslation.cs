using Dafny = Microsoft.Dafny;

namespace AST_new.Translation;

public partial class DafnyASTTranslator {
  private Statement TranslateStatement(Dafny.Statement s) {
    return s switch {
      Dafny.BlockStmt bs => TranslateBlockStmt(bs),
      Dafny.VarDeclStmt vds => TranslateVarDeclStmt(vds),
      Dafny.UpdateStmt us => TranslateUpdateStmt(us),
      Dafny.AssignStmt ass => TranslateAssignStmt(ass),
      Dafny.CallStmt cs => TranslateCallStmt(cs),
      Dafny.PrintStmt ps => TranslatePrintStmt(ps),
      Dafny.ReturnStmt rs => TranslateReturnStmt(rs),
      _ => throw new UnsupportedTranslationException(s),
    };
  }

  private BlockStmt TranslateBlockStmt(Dafny.BlockStmt bs) {
    return new BlockStmt(bs.Body.Select(TranslateStatement));
  }

  private PrintStmt TranslatePrintStmt(Dafny.PrintStmt ps) {
    return new PrintStmt(ps.Args.Select(TranslateExpression));
  }

  private ReturnStmt TranslateReturnStmt(Dafny.ReturnStmt rs) {
    return new ReturnStmt(TranslateUpdateStmt(rs.HiddenUpdate));
  }

  private VarDeclStmt TranslateVarDeclStmt(Dafny.VarDeclStmt vds) {
    return new VarDeclStmt(
      vars: vds.Locals.Select(TranslateLocalVar),
      initialiser: vds.Update == null ? null :
      (UpdateStmt)TranslateStatement(vds.Update));
  }

  private UpdateStmt TranslateUpdateStmt(Dafny.UpdateStmt us) {
    if (us.ResolvedStatements == null) {
      throw new InvalidASTOperationException(
        $"Translation requires update statement to be resolved.");
    }
    if (us.ResolvedStatements.All(s => s is Dafny.AssignStmt)) {
      return TranslateAssignStmt(
        us.ResolvedStatements.Select(s => (Dafny.AssignStmt)s));
    } else if (us.ResolvedStatements.Count() == 1
      && us.ResolvedStatements[0] is Dafny.CallStmt) {
      return TranslateCallStmt((Dafny.CallStmt)us.ResolvedStatements[0]);
    } else {
      throw new UnsupportedTranslationException(us);
    }
  }

  private UpdateStmt TranslateCallStmt(Dafny.CallStmt callStmt) {
    return new CallStmt(new MethodCallRhs(
      callee: TranslateMemberSelectExpr(callStmt.MethodSelect),
      arguments: callStmt.Args.Select(TranslateExpression)));
  }

  private AssignStmt TranslateAssignStmt(Dafny.AssignStmt ass) {
    return TranslateAssignStmt(new[] { ass });
  }

  private AssignStmt TranslateAssignStmt(IEnumerable<Dafny.AssignStmt> ass) {
    return new AssignStmt(ass.Select(a => new AssignmentPair(
      TranslateExpression(a.Lhs), TranslateAssignmentRhs(a.Rhs))));
  }
}
