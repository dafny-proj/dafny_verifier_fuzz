namespace Fuzzer;

public class LoopUnroll {
  public class Writer : ILoopWriter {

    private RandomGenerator Rand;

    public Writer(RandomGenerator? rand = null) {
      Rand = rand ?? new RandomGenerator();
    }

    public bool CanWriteLoop(ALoop loop) {
      // TODO: non-guarded and index-based loops.
      return loop is AConditionalLoop;
    }

    public Node WriteLoop(ALoop loop) {
      if (!CanWriteLoop(loop)) {
        throw new NotSupportedException($"LoopUnroll.Writer: Loop unrolling not handled.");
      }
      if (loop is AConditionalLoop cl) {
        return UnrollConditionalLoop(cl);
      }
      throw new NotImplementedException();
    }

    // TODO:
    // - Implement cloning
    // - Refactor usage of while loop writer
    // - Add invariant asserts
    private Node UnrollConditionalLoop(AConditionalLoop loop) {
      IfStmt ifs = new IfStmt(loop.Condition.Clone(), loop.Body.Clone());
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