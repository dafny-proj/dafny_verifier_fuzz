using System.Diagnostics.Contracts;

namespace Fuzzer;

public static class WhileLoop {
  public class Parser : ILoopParser {
    public bool CanParseLoop(Node node) {
      return node is WhileStmt;
    }

    public Loop ParseLoop(Node node) {
      if (!CanParseLoop(node)) {
        throw new NotSupportedException($"WhileLoop.Parser: Node of type '{node.GetType()}' cannot be parsed as a while loop.");
      }
      WhileStmt ws = (WhileStmt)node;
      LoopGuard guard;
      if (ws.Guard == null) {
        guard = new NoLoopGuard();
      } else {
        guard = new ConditionalLoopGuard(ws.Guard);
      }
      LoopBody body;
      if (ws.Body == null) {
        body = new NoLoopBody();
      } else {
        body = new BlockLoopBody(ws.Body);
      }
      LoopSpec spec = new LoopSpec(ws.Invariants, ws.Modifies, ws.AllDecreases);
      return new Loop(guard, body, spec);
    }
  }

  public class Writer : ILoopWriter {
    public bool CanWriteLoop(Loop loop) {
      return true;
    }

    public Node WriteLoop(Loop loop) {
      if (!CanWriteLoop(loop)) {
        throw new NotSupportedException($"WhileLoop.Writer: Loop cannot be represented as while loop.");
      }
      return loop.Guard switch {
        NoLoopGuard => WriteNoGuardLoop(loop),
        ConditionalLoopGuard => WriteConditionalLoop(loop),
        IndexLoopGuard => WriteIndexBasedLoop(loop),
        _ => throw new NotSupportedException("WhileLoop.Writer: Unhandled loop guard type.")
      };
    }

    private Node WriteLoop(Expression? guard, Loop loop) {
      var body = loop.Body is NoLoopBody ? null : ((BlockLoopBody)loop.Body).Block;
      return new WhileStmt(guard, body, loop.Spec.Invariants, loop.Spec.Modifies, loop.Spec.Decreases);
    }

    private Node WriteNoGuardLoop(Loop loop) {
      Contract.Requires(loop.Guard is NoLoopGuard);
      return WriteLoop(null, loop);
    }

    private Node WriteConditionalLoop(Loop loop) {
      Contract.Requires(loop.Guard is ConditionalLoopGuard);
      var guard = ((ConditionalLoopGuard)loop.Guard).Condition;
      return WriteLoop(guard, loop);
    }

    // Assumes that index has not been declared yet.
    private Node WriteIndexBasedLoop(Loop loop) {
      Contract.Requires(loop.Guard is IndexLoopGuard);
      IndexLoopGuard iGuard = (IndexLoopGuard)loop.Guard;  
      // Create variable declaration for index.
      // `var index := start`
      string indexName = iGuard.Index.Name;
      Type indexType = iGuard.Index.Type;
      LocalVariable index = new LocalVariable(indexName, indexType);
      IdentifierExpr indexIdent = new IdentifierExpr(indexName, indexType);
      ExprRhs indexInit = new ExprRhs(iGuard.Start);
      VarDeclStmt indexDecl 
        = new VarDeclStmt(index, new UpdateStmt(indexIdent, indexInit));

      // Create guard for while loop.
      Expression wGuard;
      if (iGuard.End == null) {
        // non-deterministic loop 
        // `while true`
        wGuard = new BoolLiteralExpr(true);
      } else {
        // TODO: extract iGuard.End into another variable?
        // `while index != end`
        wGuard = new BinaryExpr(BinaryExpr.Opcode.Neq, indexIdent, iGuard.End);
      }

      // Create index update statement.
      // `index := index +/- 1`
      BinaryExpr.Opcode indexUpdateOp
        = iGuard.Up ? BinaryExpr.Opcode.Add : BinaryExpr.Opcode.Sub;
      UpdateStmt indexUpdate = new UpdateStmt(
        indexIdent,
        new ExprRhs(
          new BinaryExpr(indexUpdateOp, indexIdent, new IntLiteralExpr(1))));

      // TODO: implement cloning
      // Compose body of while loop as original body and index update statement.
      BlockStmt wBody = new BlockStmt();
      if (loop.Body is BlockLoopBody b) {
        wBody.Append(b.Block.Body);
      }
      if (iGuard.Up) {
        wBody.Append(indexUpdate);
      } else {
        wBody.Prepend(indexUpdate);
      }

      // Create while loop.
      WhileStmt ws = new WhileStmt(
        wGuard,
        wBody,
        loop.Spec.Invariants,
        loop.Spec.Modifies,
        loop.Spec.Decreases
      );

      // Compose index declaration and index-based while loop.
      BlockStmt res = new BlockStmt(
        new List<Statement>() { indexDecl, ws });

      return res;
    }

  }
}