namespace Fuzzer.Tests;

[TestClass]
public class LoopRewriteMutationTest {
  [TestMethod]
  public void WriteConditionalLoopAsWhileLoop() {
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

  [TestMethod]
  public void WriteIndexBasedLoopGoingUpAsForLoop() {
    var finder = new LoopRewriteMutationFinder();
    var forUp = """
    method Fibonacci(n: nat) returns (x: nat)
    {
      x := 0;
      var y := 1;
      for i: nat := 0 to n
      {
        x, y := y, x + y;
      }
    }
    """;
    var programDafny = DafnyW.ParseDafnyProgramFromString(forUp);
    DafnyW.ResolveDafnyProgram(programDafny);
    var program = Program.FromDafny(programDafny);
    finder.FindMutations(program);
    Assert.AreEqual(1, finder.NumMutationsFound);

    var loopMutation = finder.Mutations[0];
    var originalLoop = loopMutation.OriginalLoop;
    var parser = new ForLoop.Parser();
    Assert.IsTrue(parser.CanParseLoop(originalLoop));

    var parsedLoop = parser.ParseLoop(originalLoop);
    var writer = new ForLoop.Writer();
    Assert.IsTrue(writer.CanWriteLoop(parsedLoop));

    var rewrittenLoop = writer.WriteLoop(parsedLoop);
    finder.Mutations[0].RewriteLoop(rewrittenLoop);
    var mutant = Printer.ProgramToString(program).TrimEnd();
    Assert.AreEqual(forUp, mutant);
  }

  [TestMethod]
  public void WriteIndexBasedLoopGoingDownAsForLoop() {
    var finder = new LoopRewriteMutationFinder();
    var forDown = """
    method Fibonacci(n: nat) returns (x: nat)
    {
      x := 0;
      var y := 1;
      for i: nat := n downto 0
      {
        x, y := y, x + y;
      }
    }
    """;
    var programDafny = DafnyW.ParseDafnyProgramFromString(forDown);
    DafnyW.ResolveDafnyProgram(programDafny);
    var program = Program.FromDafny(programDafny);
    finder.FindMutations(program);
    Assert.AreEqual(1, finder.NumMutationsFound);

    var loopMutation = finder.Mutations[0];
    var originalLoop = loopMutation.OriginalLoop;
    var parser = new ForLoop.Parser();
    Assert.IsTrue(parser.CanParseLoop(originalLoop));

    var parsedLoop = parser.ParseLoop(originalLoop);
    var writer = new ForLoop.Writer();
    Assert.IsTrue(writer.CanWriteLoop(parsedLoop));

    var rewrittenLoop = writer.WriteLoop(parsedLoop);
    finder.Mutations[0].RewriteLoop(rewrittenLoop);
    var mutant = Printer.ProgramToString(program).TrimEnd();
    Assert.AreEqual(forDown, mutant);
  }

  [TestMethod]
  public void WriteIndexBasedLoopWithNullUpperBoundAsForLoop() {
    var finder = new LoopRewriteMutationFinder();
    var sourceStr = """
    method BuggyFibonacci(n: nat) returns (x: nat)
      decreases *
    {
      x := 0;
      var y := 1;
      for i: nat := 0 to *
      {
        x, y := y, x + y;
      }
    }
    """;
    var programDafny = DafnyW.ParseDafnyProgramFromString(sourceStr);
    DafnyW.ResolveDafnyProgram(programDafny);
    var program = Program.FromDafny(programDafny);
    finder.FindMutations(program);
    Assert.AreEqual(1, finder.NumMutationsFound);

    var loopMutation = finder.Mutations[0];
    var originalLoop = loopMutation.OriginalLoop;
    var parser = new ForLoop.Parser();
    Assert.IsTrue(parser.CanParseLoop(originalLoop));

    var parsedLoop = parser.ParseLoop(originalLoop);
    var writer = new ForLoop.Writer();
    Assert.IsTrue(writer.CanWriteLoop(parsedLoop));

    var rewrittenLoop = writer.WriteLoop(parsedLoop);
    finder.Mutations[0].RewriteLoop(rewrittenLoop);
    var mutant = Printer.ProgramToString(program).TrimEnd();
    Assert.AreEqual(sourceStr, mutant);
  }

  [TestMethod]
  public void WriteIndexBasedLoopWithNullLowerBoundAsForLoop() {
    var finder = new LoopRewriteMutationFinder();
    var sourceStr = """
    method BuggyFibonacci(n: nat) returns (x: nat)
      decreases *
    {
      x := 0;
      var y := 1;
      for i: nat := n downto *
      {
        x, y := y, x + y;
      }
    }
    """;
    var programDafny = DafnyW.ParseDafnyProgramFromString(sourceStr);
    DafnyW.ResolveDafnyProgram(programDafny);
    var program = Program.FromDafny(programDafny);
    finder.FindMutations(program);
    Assert.AreEqual(1, finder.NumMutationsFound);

    var loopMutation = finder.Mutations[0];
    var originalLoop = loopMutation.OriginalLoop;
    var parser = new ForLoop.Parser();
    Assert.IsTrue(parser.CanParseLoop(originalLoop));

    var parsedLoop = parser.ParseLoop(originalLoop);
    var writer = new ForLoop.Writer();
    Assert.IsTrue(writer.CanWriteLoop(parsedLoop));

    var rewrittenLoop = writer.WriteLoop(parsedLoop);
    finder.Mutations[0].RewriteLoop(rewrittenLoop);
    var mutant = Printer.ProgramToString(program).TrimEnd();
    Assert.AreEqual(sourceStr, mutant);
  }

  [TestMethod]
  public void WriteIndexBasedLoopGoingUpAsWhileLoop() {
    var finder = new LoopRewriteMutationFinder();
    var forUp = """
    method Fibonacci(n: nat) returns (x: nat)
    {
      x := 0;
      var y := 1;
      for i: nat := 0 to n
      {
        x, y := y, x + y;
      }
    }
    """;
    var programDafny = DafnyW.ParseDafnyProgramFromString(forUp);
    DafnyW.ResolveDafnyProgram(programDafny);
    var program = Program.FromDafny(programDafny);
    finder.FindMutations(program);
    Assert.AreEqual(1, finder.NumMutationsFound);

    var loopMutation = finder.Mutations[0];
    var originalLoop = loopMutation.OriginalLoop;
    var parser = new ForLoop.Parser();
    Assert.IsTrue(parser.CanParseLoop(originalLoop));

    var parsedLoop = parser.ParseLoop(originalLoop);
    var writer = new WhileLoop.Writer();
    Assert.IsTrue(writer.CanWriteLoop(parsedLoop));

    var rewrittenLoop = writer.WriteLoop(parsedLoop);
    finder.Mutations[0].RewriteLoop(rewrittenLoop);
    var mutant = Printer.ProgramToString(program).TrimEnd();
    var whileUp = """
    method Fibonacci(n: nat) returns (x: nat)
    {
      x := 0;
      var y := 1;
      var i: nat := 0;
      while i != n
      {
        x, y := y, x + y;
        i := i + 1;
      }
    }
    """;
    Assert.AreEqual(whileUp, mutant);
  }

  [TestMethod]
  public void WriteIndexBasedLoopGoingDownAsWhileLoop() {
    var finder = new LoopRewriteMutationFinder();
    var forUp = """
    method Fibonacci(n: nat) returns (x: nat)
    {
      x := 0;
      var y := 1;
      for i: nat := n downto 0
      {
        x, y := y, x + y;
      }
    }
    """;
    var programDafny = DafnyW.ParseDafnyProgramFromString(forUp);
    DafnyW.ResolveDafnyProgram(programDafny);
    var program = Program.FromDafny(programDafny);
    finder.FindMutations(program);
    Assert.AreEqual(1, finder.NumMutationsFound);

    var loopMutation = finder.Mutations[0];
    var originalLoop = loopMutation.OriginalLoop;
    var parser = new ForLoop.Parser();
    Assert.IsTrue(parser.CanParseLoop(originalLoop));

    var parsedLoop = parser.ParseLoop(originalLoop);
    var writer = new WhileLoop.Writer();
    Assert.IsTrue(writer.CanWriteLoop(parsedLoop));

    var rewrittenLoop = writer.WriteLoop(parsedLoop);
    finder.Mutations[0].RewriteLoop(rewrittenLoop);
    var mutant = Printer.ProgramToString(program).TrimEnd();
    var whileDown = """
    method Fibonacci(n: nat) returns (x: nat)
    {
      x := 0;
      var y := 1;
      var i: nat := n;
      while i != 0
      {
        i := i - 1;
        x, y := y, x + y;
      }
    }
    """;
    Assert.AreEqual(whileDown, mutant);
  }

  [TestMethod]
  public void WriteIndexBasedLoopWithNullUpperBoundAsWhileLoop() {
    var finder = new LoopRewriteMutationFinder();
    var forUp = """
    method BuggyFibonacci(n: nat) returns (x: nat)
      decreases *
    {
      x := 0;
      var y := 1;
      for i: nat := 0 to *
      {
        x, y := y, x + y;
      }
    }
    """;
    var programDafny = DafnyW.ParseDafnyProgramFromString(forUp);
    DafnyW.ResolveDafnyProgram(programDafny);
    var program = Program.FromDafny(programDafny);
    finder.FindMutations(program);
    Assert.AreEqual(1, finder.NumMutationsFound);

    var loopMutation = finder.Mutations[0];
    var originalLoop = loopMutation.OriginalLoop;
    var parser = new ForLoop.Parser();
    Assert.IsTrue(parser.CanParseLoop(originalLoop));

    var parsedLoop = parser.ParseLoop(originalLoop);
    var writer = new WhileLoop.Writer();
    Assert.IsTrue(writer.CanWriteLoop(parsedLoop));

    var rewrittenLoop = writer.WriteLoop(parsedLoop);
    finder.Mutations[0].RewriteLoop(rewrittenLoop);
    var mutant = Printer.ProgramToString(program).TrimEnd();
    var whileDown = """
    method BuggyFibonacci(n: nat) returns (x: nat)
      decreases *
    {
      x := 0;
      var y := 1;
      var i: nat := 0;
      while true
      {
        x, y := y, x + y;
        i := i + 1;
      }
    }
    """;
    Assert.AreEqual(whileDown, mutant);
  }

  [TestMethod]
  public void WriteIndexBasedLoopWithNullLowerBoundAsWhileLoop() {
    var finder = new LoopRewriteMutationFinder();
    var forUp = """
    method BuggyFibonacci(n: nat) returns (x: nat)
      decreases *
    {
      x := 0;
      var y := 1;
      for i: nat := n downto *
      {
        x, y := y, x + y;
      }
    }
    """;
    var programDafny = DafnyW.ParseDafnyProgramFromString(forUp);
    DafnyW.ResolveDafnyProgram(programDafny);
    var program = Program.FromDafny(programDafny);
    finder.FindMutations(program);
    Assert.AreEqual(1, finder.NumMutationsFound);

    var loopMutation = finder.Mutations[0];
    var originalLoop = loopMutation.OriginalLoop;
    var parser = new ForLoop.Parser();
    Assert.IsTrue(parser.CanParseLoop(originalLoop));

    var parsedLoop = parser.ParseLoop(originalLoop);
    var writer = new WhileLoop.Writer();
    Assert.IsTrue(writer.CanWriteLoop(parsedLoop));

    var rewrittenLoop = writer.WriteLoop(parsedLoop);
    finder.Mutations[0].RewriteLoop(rewrittenLoop);
    var mutant = Printer.ProgramToString(program).TrimEnd();
    var whileDown = """
    method BuggyFibonacci(n: nat) returns (x: nat)
      decreases *
    {
      x := 0;
      var y := 1;
      var i: nat := n;
      while true
      {
        i := i - 1;
        x, y := y, x + y;
      }
    }
    """;
    Assert.AreEqual(whileDown, mutant);
  }
}