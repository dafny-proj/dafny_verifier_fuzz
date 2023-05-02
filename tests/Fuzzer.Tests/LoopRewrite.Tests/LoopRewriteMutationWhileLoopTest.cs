namespace Fuzzer.Tests;

[TestClass]
public class LoopRewriteMutationWhileLoopTest {
  [TestMethod]
  public void WriteConditionalLoop() {
    var finder = new LoopRewriteMutationFinder();
    var sourceStr = """
    method BinarySearch(a: array<int>, len: int, key: int)
      returns (i: int)
    {
      var lo, hi := 0, len;
      while (lo < hi)
      {
        var mid := (lo + hi) / 2;
        if key < a[mid] {
          hi := mid;
        } else if a[mid] < key {
          lo := mid + 1;
        } else {
          return mid;
        }
      }
      return -1;
    }
    """;
    var programDafny = DafnyW.ParseDafnyProgramFromString(sourceStr);
    DafnyW.ResolveDafnyProgram(programDafny);
    var program = Program.FromDafny(programDafny);
    finder.FindMutations(program);
    Assert.AreEqual(1, finder.NumMutationsFound);

    var loopMutation = finder.Mutations[0];
    var originalLoop = loopMutation.OriginalLoop; 
    var parser = new WhileLoop.Parser();
    Assert.IsTrue(parser.CanParseLoop(originalLoop));

    var parsedLoop = parser.ParseLoop(originalLoop);
    var writer = new WhileLoop.Writer();
    Assert.IsTrue(writer.CanWriteLoop(parsedLoop));

    var rewrittenLoop = writer.WriteLoop(parsedLoop);
    finder.Mutations[0].RewriteLoop(rewrittenLoop);
    var mutant = Printer.ProgramToString(program);
    var expected = sourceStr;
    Assert.AreEqual(expected, mutant.TrimEnd());
  }
}