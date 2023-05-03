using System.Diagnostics.Contracts;

namespace Fuzzer;

public static class ForLoop {
  public class Parser : ILoopParser {
    public bool CanParseLoop(Node node) {
      return node is ForLoopStmt;
    }

    public Loop ParseLoop(Node node) {
      if (!CanParseLoop(node)) {
        throw new NotSupportedException($"ForLoop.Parser: Node of type '{node.GetType()}' cannot be parsed as a for loop.");
      }
      ForLoopStmt fs = (ForLoopStmt)node;
      IndexLoopGuard guard = new IndexLoopGuard(fs.LoopIndex, fs.Start, fs.End, fs.GoingUp);
      // TODO: extract common loop body and spec processing
      LoopBody body;
      if (fs.Body == null) {
        body = new NoLoopBody();
      } else {
        body = new BlockLoopBody(fs.Body);
      }
      LoopSpec spec = new LoopSpec(fs.Invariants, fs.Modifies, fs.AllDecreases);
      return new Loop(guard, body, spec);
    }
  }

  public class Writer : ILoopWriter {
    public bool CanWriteLoop(Loop loop) {
      return loop.Guard is IndexLoopGuard;
    }

    public Node WriteLoop(Loop loop) {
      if (!CanWriteLoop(loop)) {
        throw new NotSupportedException($"ForLoop.Writer: Loop cannot be represented as for loop.");
      }
      IndexLoopGuard guard = (IndexLoopGuard)loop.Guard;
      var body = loop.Body is NoLoopBody ? null : ((BlockLoopBody)loop.Body).Block;
      return new ForLoopStmt(guard.Index, guard.Start, guard.End, guard.Up, body, loop.Spec.Invariants, loop.Spec.Modifies, loop.Spec.Decreases);
    }

  }
}