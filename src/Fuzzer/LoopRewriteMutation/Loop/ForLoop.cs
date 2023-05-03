using System.Diagnostics.Contracts;

namespace Fuzzer;

public static class ForLoop {
  public class Parser : ILoopParser {
    public bool CanParseLoop(Node node) {
      return node is ForLoopStmt fs && fs.Body != null;
    }

    public ALoop ParseLoop(Node node) {
      if (!CanParseLoop(node)) {
        throw new NotSupportedException($"ForLoop.Parser: Node of type '{node.GetType()}' cannot be parsed as a for loop.");
      }
      ForLoopStmt fs = (ForLoopStmt)node;
      Contract.Assert(fs.Body != null);
      return new AIndexLoop(
        fs.LoopIndex, fs.Start, fs.End, fs.GoingUp,
        fs.Body, fs.Invariants, fs.Modifies, fs.AllDecreases
      );
    }
  }

  public class Writer : ILoopWriter {
    public bool CanWriteLoop(ALoop loop) {
      return loop is AIndexLoop;
    }

    public Node WriteLoop(ALoop loop) {
      if (!CanWriteLoop(loop)) {
        throw new NotSupportedException($"ForLoop.Writer: Loop cannot be represented as for loop.");
      }
      AIndexLoop iLoop = (AIndexLoop)loop;
      return new ForLoopStmt(
        iLoop.Index, iLoop.IStart, iLoop.IEnd, iLoop.Up,
        iLoop.Body, iLoop.Invariants, iLoop.Modifies, iLoop.Decreases
      );
    }

  }
}