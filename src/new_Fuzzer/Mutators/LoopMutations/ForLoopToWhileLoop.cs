namespace Fuzzer_new;

// Requires loops to have bodies.
/// <summary>
/// `for i := s to e { body }`
/// Rewrites to
/// ```
/// var i := s;
/// while (i != e) { body; i := i + 1; }
/// ```
/// </summary>
public partial class ForLoopToWhileLoopRewriter {
  private ForLoopStmt _forLoop;
  private BlockStmt _forLoopParent;

  public ForLoopToWhileLoopRewriter(
  ForLoopStmt forLoop, BlockStmt forLoopParent) {
    _forLoop = forLoop;
    _forLoopParent = forLoopParent;
  }

  private void Rewrite() {
    var indexBV = _forLoop.LoopIndex;
    var indexLV = new LocalVar(indexBV.Name, indexBV.Type, indexBV.ExplicitType);
    // Create variable declaration for index.
    // `var index := start`
    var indexInit = new AssignStmt(new AssignmentPair(
      new IdentifierExpr(indexLV), new ExprRhs(_forLoop.LoopStart)));
    var indexDecl = new VarDeclStmt(var: indexLV, initialiser: indexInit);

    // Create guard for while loop.
    Expression guard;
    if (_forLoop.LoopEnd == null) {
      // non-deterministic loop: `while true`
      guard = new BoolLiteralExpr(true);
    } else {
      // `while index != end`
      guard = new BinaryExpr(
        BinaryExpr.Opcode.Neq, new IdentifierExpr(indexLV), _forLoop.LoopEnd);
    }

    // Create index update statement.
    // `index := index +/- 1`
    var nextIndex = new BinaryExpr(
      _forLoop.GoesUp ? BinaryExpr.Opcode.Add : BinaryExpr.Opcode.Sub,
      new IdentifierExpr(indexLV), new IntLiteralExpr(1));
    var indexUpdate = new AssignStmt(new AssignmentPair(
      new IdentifierExpr(indexLV), new ExprRhs(nextIndex)));

    // Compose body of while loop as original body and index update statement.
    var body = _forLoop.Body!;
    if (_forLoop.GoesUp) {
      body.Append(indexUpdate);
    } else {
      body.Prepend(indexUpdate);
    }

    // Create while loop.
    var whileLoop = new WhileLoopStmt(guard: guard, body: body,
      inv: _forLoop.Invariants, mod: _forLoop.Modifies, dec: _forLoop.Decreases);

    // Replace for loop with index declaration and while loop.
    _forLoopParent.Replace(_forLoop, new Statement[] { indexDecl, whileLoop });
  }

}