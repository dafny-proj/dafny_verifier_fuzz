using System.Diagnostics.Contracts;

namespace Fuzzer;

public static class WhileLoop {
  public class Parser : ILoopParser {
    public bool CanParseLoop(Node node) {
      return node is WhileStmt ws && ws.Body != null;
    }

    public ALoop ParseLoop(Node node) {
      if (!CanParseLoop(node)) {
        throw new NotSupportedException($"WhileLoop.Parser: Node of type '{node.GetType()}' cannot be parsed as a while loop.");
      }
      WhileStmt ws = (WhileStmt)node;
      Contract.Assert(ws.Body != null);
      return new AConditionalLoop(
        ws.Guard, ws.Body, ws.Invariants, ws.Modifies, ws.AllDecreases
      );
    }
  }

  public class Writer : ILoopWriter {
    public bool CanWriteLoop(ALoop loop) {
      return true;
    }

    public Node WriteLoop(ALoop loop) {
      if (!CanWriteLoop(loop)) {
        throw new NotSupportedException($"WhileLoop.Writer: Loop cannot be represented as while loop.");
      }
      return loop switch {
        AConditionalLoop l => WriteConditionalLoop(l),
        AIndexLoop l => WriteIndexLoop(l),
        _ => throw new NotSupportedException("WhileLoop.Writer: Unhandled loop type.")
      };
    }

    private Node WriteLoop(Expression? guard, ALoop loop) {
      var body = loop.Body;
      return new WhileStmt(guard, body, loop.Invariants, loop.Modifies, loop.Decreases);
    }

    private Node WriteConditionalLoop(AConditionalLoop loop) {
      return WriteLoop(loop.Condition, loop);
    }

    // Assumes that index has not been declared yet.
    private Node WriteIndexLoop(AIndexLoop loop) {
      // Create variable declaration for index.
      // `var index := start`
      string indexName = loop.Index.Name;
      Type indexType = loop.Index.Type;
      LocalVariable index = new LocalVariable(indexName, indexType);
      IdentifierExpr indexIdent = new IdentifierExpr(indexName, indexType);
      ExprRhs indexInit = new ExprRhs(loop.IStart);
      VarDeclStmt indexDecl
        = new VarDeclStmt(index, new UpdateStmt(indexIdent, indexInit));

      // Create guard for while loop.
      Expression wGuard;
      if (loop.IEnd == null) {
        // non-deterministic loop 
        // `while true`
        wGuard = new BoolLiteralExpr(true);
      } else {
        // TODO: extract iGuard.End into another variable?
        // `while index != end`
        wGuard = new BinaryExpr(BinaryExpr.Opcode.Neq, indexIdent, loop.IEnd);
      }

      // Create index update statement.
      // `index := index +/- 1`
      BinaryExpr.Opcode indexUpdateOp
        = loop.Up ? BinaryExpr.Opcode.Add : BinaryExpr.Opcode.Sub;
      UpdateStmt indexUpdate = new UpdateStmt(
        indexIdent,
        new ExprRhs(
          new BinaryExpr(indexUpdateOp, indexIdent, new IntLiteralExpr(1))));

      // Compose body of while loop as original body and index update statement.
      BlockStmt wBody = loop.Body.Clone();
      if (loop.Up) {
        wBody.Append(indexUpdate);
      } else {
        wBody.Prepend(indexUpdate);
      }

      // Create while loop.
      WhileStmt ws = new WhileStmt(
        wGuard,
        wBody,
        loop.Invariants,
        loop.Modifies,
        loop.Decreases
      );

      // Compose index declaration and index-based while loop.
      BlockStmt res = new BlockStmt(
        new List<Statement>() { indexDecl, ws });

      return res;
    }

  }
}