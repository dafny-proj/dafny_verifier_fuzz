namespace MutantGenerator.Tests;

[TestClass]
public class ForLoopToWhileLoopTest {
  // Test single loop conversion to while loop.
  public void TestForLoopToWhileLoop(string forLoop, string whileLoop) {
    var program = DafnyW.ParseProgramFromString(forLoop);
    var mutator = new ForLoopToWhileLoopMutator(new MockRandomizer());
    var potentialMutations = mutator.FindPotentialMutations(program);
    Assert.AreEqual(1, potentialMutations.Count);

    var mutation = potentialMutations[0];
    mutator.ApplyMutation(mutation);
    var mutant = ASTPrinter.PrintNodeToString(program).TrimEnd();
    Assert.AreEqual(whileLoop, mutant);
  }

  [TestMethod]
  public void ForLoopGoingUpToWhileLoop() {
    var f = """
    method Fibonacci(n: nat) returns (x: nat) {
      x := 0;
      var y := 1;
      for i: nat := 0 to n {
        x, y := y, x + y;
      }
    }
    """;
    var w = """
    method Fibonacci(n: nat) returns (x: nat) {
      x := 0;
      var y := 1;
      var i: nat := 0;
      while i != n {
        x, y := y, x + y;
        i := i + 1;
      }
    }
    """;
    TestForLoopToWhileLoop(f, w);
  }

  [TestMethod]
  public void ForLoopGoingDownToWhileLoop() {
    var f = """
    method Fibonacci(n: nat) returns (x: nat) {
      x := 0;
      var y := 1;
      for i: nat := n downto 0 {
        x, y := y, x + y;
      }
    }
    """;
    var w = """
    method Fibonacci(n: nat) returns (x: nat) {
      x := 0;
      var y := 1;
      var i: nat := n;
      while i != 0 {
        i := i - 1;
        x, y := y, x + y;
      }
    }
    """;
    TestForLoopToWhileLoop(f, w);
  }

  [TestMethod]
  public void ForLoopNullUpperBoundToWhileLoop() {
    var f = """
    method BuggyFibonacci(n: nat) returns (x: nat)
      decreases *
    {
      x := 0;
      var y := 1;
      for i: nat := 0 to * {
        x, y := y, x + y;
      }
    }
    """;
    var w = """
    method BuggyFibonacci(n: nat) returns (x: nat)
      decreases *
    {
      x := 0;
      var y := 1;
      var i: nat := 0;
      while true {
        x, y := y, x + y;
        i := i + 1;
      }
    }
    """;
    TestForLoopToWhileLoop(f, w);
  }

  [TestMethod]
  public void ForLoopNullLowerBoundToWhileLoop() {
    var f = """
    method BuggyFibonacci(n: nat) returns (x: nat)
      decreases *
    {
      x := 0;
      var y := 1;
      for i: nat := n downto * {
        x, y := y, x + y;
      }
    }
    """;
    var w = """
    method BuggyFibonacci(n: nat) returns (x: nat)
      decreases *
    {
      x := 0;
      var y := 1;
      var i: nat := n;
      while true {
        i := i - 1;
        x, y := y, x + y;
      }
    }
    """;
    TestForLoopToWhileLoop(f, w);
  }
}