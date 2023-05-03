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

    private Node WriteIndexBasedLoop(Loop loop) {
      Contract.Requires(loop.Guard is IndexLoopGuard);
      throw new NotImplementedException();
    }

  }
}