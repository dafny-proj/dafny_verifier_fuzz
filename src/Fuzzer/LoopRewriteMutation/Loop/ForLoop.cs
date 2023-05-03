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
      var lo = fs.GoingUp ? fs.Start : fs.End;
      var hi = fs.GoingUp ? fs.End : fs.Start;
      IndexLoopGuard guard = new IndexLoopGuard(fs.LoopIndex, lo, hi);
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
    private RandomGenerator Rand;

    public Writer(RandomGenerator? rand = null) {
      Rand = rand ?? new RandomGenerator();
    }

    public bool CanWriteLoop(Loop loop) {
      return loop.Guard is IndexLoopGuard;
    }

    public Node WriteLoop(Loop loop) {
      if (!CanWriteLoop(loop)) {
        throw new NotSupportedException($"ForLoop.Writer: Loop cannot be represented as for loop.");
      }
      IndexLoopGuard guard = (IndexLoopGuard)loop.Guard;
      bool up;
      if (guard.Lo == null) {
        up = false;
      } else if (guard.Hi == null) {
        up = true;
      } else {
        up = Rand.Bool();
      }
      var start = up ? guard.Lo : guard.Hi;
      var end = up ? guard.Hi : guard.Lo;
      var body = loop.Body is NoLoopBody ? null : ((BlockLoopBody)loop.Body).Block;
      Contract.Assert(start != null);
      return new ForLoopStmt(guard.Index, start, end, up, body, loop.Spec.Invariants, loop.Spec.Modifies, loop.Spec.Decreases);
    }

  }
}