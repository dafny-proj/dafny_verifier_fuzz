namespace Fuzzer_new;

public partial class ForLoopToWhileLoopMutationRewriter {
  private ForLoopStmt forLoop;
  private BlockStmt enclosingScope;

  public ForLoopToWhileLoopMutationRewriter(ForLoopToWhileLoopMutation m)
  : this(m.forLoop, m.enclosingScope) { }

  public ForLoopToWhileLoopMutationRewriter(
  ForLoopStmt forLoop, BlockStmt enclosingScope) {
    this.forLoop = forLoop;
    this.enclosingScope = enclosingScope;
  }

  public void Rewrite() {
    var indexBV = forLoop.LoopIndex;
    var indexLV = new LocalVar(indexBV.Name, indexBV.Type, indexBV.ExplicitType);
    // Create variable declaration for index.
    // `var index := start`
    var indexInit = new AssignStmt(new AssignmentPair(
      new IdentifierExpr(indexLV), new ExprRhs(forLoop.LoopStart)));
    var indexDecl = new VarDeclStmt(var: indexLV, initialiser: indexInit);

    // Create guard for while loop.
    Expression guard;
    if (forLoop.LoopEnd == null) {
      // non-deterministic loop: `while true`
      guard = new BoolLiteralExpr(true);
    } else {
      // `while index != end`
      guard = new BinaryExpr(
        BinaryExpr.Opcode.Neq, new IdentifierExpr(indexLV), forLoop.LoopEnd);
    }

    // Create index update statement.
    // `index := index +/- 1`
    var nextIndex = new BinaryExpr(
      forLoop.GoesUp ? BinaryExpr.Opcode.Add : BinaryExpr.Opcode.Sub,
      new IdentifierExpr(indexLV), new IntLiteralExpr(1));
    var indexUpdate = new AssignStmt(new AssignmentPair(
      new IdentifierExpr(indexLV), new ExprRhs(nextIndex)));

    // Compose body of while loop as original body and index update statement.
    var body = forLoop.Body!;
    if (forLoop.GoesUp) {
      body.Append(indexUpdate);
    } else {
      body.Prepend(indexUpdate);
    }

    // Create while loop.
    var whileLoop = new WhileLoopStmt(guard: guard, body: body,
      inv: forLoop.Invariants, mod: forLoop.Modifies, dec: forLoop.Decreases);

    // Replace for loop with index declaration and while loop.
    enclosingScope.Replace(forLoop, new Statement[] { indexDecl, whileLoop });
  }

}