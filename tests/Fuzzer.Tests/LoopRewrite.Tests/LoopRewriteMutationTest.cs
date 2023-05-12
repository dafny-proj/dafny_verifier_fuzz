namespace Fuzzer.Tests;

[TestClass]
public class LoopRewriteMutationTest {
  [TestMethod]
  public void WriteConditionalLoopAsWhileLoop() {
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
    var parentMap = new ParentMap(program);
    var finder = new LoopRewriteMutationFinder(parentMap);
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
    var parentMap = new ParentMap(program);
    var finder = new LoopRewriteMutationFinder(parentMap);
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
    var parentMap = new ParentMap(program);
    var finder = new LoopRewriteMutationFinder(parentMap);
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
    var parentMap = new ParentMap(program);
    var finder = new LoopRewriteMutationFinder(parentMap);
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
    var parentMap = new ParentMap(program);
    var finder = new LoopRewriteMutationFinder(parentMap);
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
    var parentMap = new ParentMap(program);
    var finder = new LoopRewriteMutationFinder(parentMap);
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
    var parentMap = new ParentMap(program);
    var finder = new LoopRewriteMutationFinder(parentMap);
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
    var parentMap = new ParentMap(program);
    var finder = new LoopRewriteMutationFinder(parentMap);
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
    var parentMap = new ParentMap(program);
    var finder = new LoopRewriteMutationFinder(parentMap);
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

  public void TestLoopUnroll(
    string input,
    string expectedOutput,
    int expectedNumMutationsFound = 1,
    int mutationToTrigger = 0
  ) {
    var programDafny = DafnyW.ParseDafnyProgramFromString(input);
    DafnyW.ResolveDafnyProgram(programDafny);
    var program = Program.FromDafny(programDafny);
    var parentMap = new ParentMap(program);
    var finder = new LoopRewriteMutationFinder(parentMap);
    finder.FindMutations(program);
    Assert.AreEqual(expectedNumMutationsFound, finder.NumMutationsFound);

    var loopMutation = finder.Mutations[mutationToTrigger];
    var originalLoop = loopMutation.OriginalLoop;
    var parser = new WhileLoop.Parser();
    Assert.IsTrue(parser.CanParseLoop(originalLoop));

    var parsedLoop = parser.ParseLoop(originalLoop);
    var writer = new LoopUnroll.Writer(parentMap);
    Assert.IsTrue(writer.CanWriteLoop(parsedLoop));

    var rewrittenLoop = writer.WriteLoop(parsedLoop);
    loopMutation.RewriteLoop(rewrittenLoop);
    var mutant = Printer.ProgramToString(program).TrimEnd();
    Assert.AreEqual(expectedOutput, mutant);
  }

  [TestMethod]
  public void UnrollSimpleWhileLoop() {
    var whileWhole = """
    method Foo(n: nat)
    {
      var i := 0;
      while i < n
      {
        i := i + 1;
      }
    }
    """;
    var whileUnrolled = """
    method Foo(n: nat)
    {
      var i := 0;
      if i < n {
        i := i + 1;
      }
      while i < n
      {
        i := i + 1;
      }
    }
    """;
    TestLoopUnroll(whileWhole, whileUnrolled);
  }

  [TestMethod]
  public void UnrollWhileLoopWithUnconditionalBreak() {
    var whileWhole = """
    method Foo(n: nat)
    {
      print 0;
      while *
      {
        print 1;
        break;
        print 2;
      }
      print 3;
    }
    """;
    var whileUnrolled = """
    method Foo(n: nat)
    {
      print 0;
      var breakVar: bool := false;
      label breakLabel:
      if * {
        print 1;
        breakVar := true;
        break breakLabel;
        print 2;
      }
      if !breakVar {
        while *
        {
          print 1;
          break;
          print 2;
        }
      }
      print 3;
    }
    """;
    TestLoopUnroll(whileWhole, whileUnrolled);
  }

  [TestMethod]
  public void UnrollWhileLoopWithConditionalBreak() {
    var whileWhole = """
    function Bool(n: nat): bool

    method Foo(n: nat)
    {
      print 0;
      while Bool(0)
      {
        print 1;
        if Bool(1) {
          print 2;
          break;
          print 3;
        } else {
          print 4;
        }
        print 5;
      }
      print 6;
    }
    """;
    var whileUnrolled = """
    function Bool(n: nat): bool

    method Foo(n: nat)
    {
      print 0;
      var breakVar: bool := false;
      label breakLabel:
      if Bool(0) {
        print 1;
        if Bool(1) {
          print 2;
          breakVar := true;
          break breakLabel;
          print 3;
        } else {
          print 4;
        }
        print 5;
      }
      if !breakVar {
        while Bool(0)
        {
          print 1;
          if Bool(1) {
            print 2;
            break;
            print 3;
          } else {
            print 4;
          }
          print 5;
        }
      }
      print 6;
    }
    """;
    TestLoopUnroll(whileWhole, whileUnrolled);
  }

  [TestMethod]
  public void UnrollWhileLoopWithNestedConditionalBreak() {
    var whileWhole = """
    function Bool(n: nat): bool

    method Foo(n: nat)
    {
      print 0;
      while Bool(0)
      {
        print 1;
        if Bool(1) {
          print 2;
          if Bool(2) {
            print 3;
            break;
            print 4;
          }
          print 5;
        }
        print 6;
      }
      print 7;
    }
    """;
    var whileUnrolled = """
    function Bool(n: nat): bool

    method Foo(n: nat)
    {
      print 0;
      var breakVar: bool := false;
      label breakLabel:
      if Bool(0) {
        print 1;
        if Bool(1) {
          print 2;
          if Bool(2) {
            print 3;
            breakVar := true;
            break breakLabel;
            print 4;
          }
          print 5;
        }
        print 6;
      }
      if !breakVar {
        while Bool(0)
        {
          print 1;
          if Bool(1) {
            print 2;
            if Bool(2) {
              print 3;
              break;
              print 4;
            }
            print 5;
          }
          print 6;
        }
      }
      print 7;
    }
    """;
    TestLoopUnroll(whileWhole, whileUnrolled);
  }

  [TestMethod]
  public void UnrollNestedWhileLoopWithInnerBreak() {
    var whileWhole = """
    function Bool(n: nat): bool

    method Foo(n: nat)
    {
      print 0;
      while Bool(0)
      {
        print 1;
        while Bool(1)
        {
          print 2;
          break;
          print 3;
        }
        print 4;
      }
      print 5;
    }
    """;
    var outerWhileUnrolled = """
    function Bool(n: nat): bool

    method Foo(n: nat)
    {
      print 0;
      if Bool(0) {
        print 1;
        while Bool(1)
        {
          print 2;
          break;
          print 3;
        }
        print 4;
      }
      while Bool(0)
      {
        print 1;
        while Bool(1)
        {
          print 2;
          break;
          print 3;
        }
        print 4;
      }
      print 5;
    }
    """;
    var innerWhileUnrolled = """
    function Bool(n: nat): bool

    method Foo(n: nat)
    {
      print 0;
      while Bool(0)
      {
        print 1;
        var breakVar: bool := false;
        label breakLabel:
        if Bool(1) {
          print 2;
          breakVar := true;
          break breakLabel;
          print 3;
        }
        if !breakVar {
          while Bool(1)
          {
            print 2;
            break;
            print 3;
          }
        }
        print 4;
      }
      print 5;
    }
    """;
    TestLoopUnroll(whileWhole,
      outerWhileUnrolled,
      expectedNumMutationsFound: 2,
      mutationToTrigger: 0);
    TestLoopUnroll(whileWhole,
      innerWhileUnrolled,
      expectedNumMutationsFound: 2,
      mutationToTrigger: 1);
  }

  [TestMethod]
  public void UnrollNestedWhileLoopWithInnerBreakBreak() {
    var whileWhole = """
    function Bool(n: nat): bool

    method Foo(n: nat)
    {
      print 0;
      while Bool(0)
      {
        print 1;
        while Bool(1)
        {
          print 2;
          break break;
          print 3;
        }
        print 4;
      }
      print 5;
    }
    """;
    var outerWhileUnrolled = """
    function Bool(n: nat): bool

    method Foo(n: nat)
    {
      print 0;
      var breakVar: bool := false;
      label breakLabel:
      if Bool(0) {
        print 1;
        while Bool(1)
        {
          print 2;
          breakVar := true;
          break breakLabel;
          print 3;
        }
        print 4;
      }
      if !breakVar {
        while Bool(0)
        {
          print 1;
          while Bool(1)
          {
            print 2;
            break break;
            print 3;
          }
          print 4;
        }
      }
      print 5;
    }
    """;
    var innerWhileUnrolled = """
    function Bool(n: nat): bool

    method Foo(n: nat)
    {
      print 0;
      while Bool(0)
      {
        print 1;
        if Bool(1) {
          print 2;
          break;
          print 3;
        }
        while Bool(1)
        {
          print 2;
          break break;
          print 3;
        }
        print 4;
      }
      print 5;
    }
    """;
    TestLoopUnroll(whileWhole,
      outerWhileUnrolled,
      expectedNumMutationsFound: 2,
      mutationToTrigger: 0);
    TestLoopUnroll(whileWhole,
      innerWhileUnrolled,
      expectedNumMutationsFound: 2,
      mutationToTrigger: 1);
  }

}