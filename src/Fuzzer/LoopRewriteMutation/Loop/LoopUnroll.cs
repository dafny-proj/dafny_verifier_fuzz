using System.Diagnostics.Contracts;

namespace Fuzzer;

public class LoopUnroll {
  public class Writer : ILoopWriter {

    private RandomGenerator Rand;

    public Writer(RandomGenerator? rand = null) {
      Rand = rand ?? new RandomGenerator();
    }

    public bool CanWriteLoop(Loop loop) {
      // TODO: non-guarded and index-based loops.
      return loop.Guard is ConditionalLoopGuard;
    }

    public Node WriteLoop(Loop loop) {
      if (!CanWriteLoop(loop)) {
        throw new NotSupportedException($"LoopUnroll.Writer: Loop unrolling not handled.");
      }
      if (loop.Guard is ConditionalLoopGuard) {
        return UnrollConditionalLoop(loop);
      }
      throw new NotImplementedException();
    }

    // TODO:
    // - Implement cloning
    // - Refactor usage of while loop writer
    // - Add invariant asserts
    private Node UnrollConditionalLoop(Loop loop) {
      Contract.Requires(loop.Guard is ConditionalLoopGuard);
      ConditionalLoopGuard guard = (ConditionalLoopGuard)loop.Guard;
      // Assume has body
      BlockLoopBody body = (BlockLoopBody)loop.Body;
      IfStmt ifs = new IfStmt(guard.Condition.Clone(), body.Block.Clone());
      Statement ws = (Statement)new WhileLoop.Writer().WriteLoop(loop);
      Node res;
      var nest = Rand.Bool();
      if (nest) {
        ifs.Thn.Append(ws);
        res = ifs;
      } else {
        res = new BlockStmt(new List<Statement>() { ifs, ws });
      }
      return res;
    }
  }
}