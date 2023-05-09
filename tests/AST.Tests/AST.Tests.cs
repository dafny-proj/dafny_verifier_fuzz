using Microsoft.VisualStudio.TestTools.UnitTesting;
using DafnyW = DafnyWrappers.DafnyWrappers;

namespace AST.Tests;

[TestClass]
public class ASTTests {
  private void CanParseAndPrintFeature(string sourceStr) {
    var programDafny = DafnyW.ParseDafnyProgramFromString(sourceStr);
    DafnyW.ResolveDafnyProgram(programDafny);
    var program = Program.FromDafny(programDafny);
    var outputStr = Printer.ProgramToString(program);
    Assert.AreEqual(sourceStr, outputStr.TrimEnd(), /*ignore_case=*/false);
  }

  [TestMethod]
  public void CanParseAndPrintMethod() {
    var sourceStr = """
    method Sum(x: int, y: int) returns (z: int)
    {
      z := x + y;
    }
    """;
    CanParseAndPrintFeature(sourceStr);
  }

  [TestMethod]
  public void CanParseAndPrintFunction() {
    var sourceStr = """
    function Identity(x: int): int
    {
      x
    }
    """;
    CanParseAndPrintFeature(sourceStr);
  }

  [TestMethod]
  public void CanParseAndPrintDifferentTypes() {
    var sourceStr = """
    method m()
    {
      var x: int := 1;
      var b, c: bool := true, false;
      var n: nat := 0;
      var s: string := "hello";
      var a := new int[1];
      var a2: array2<int> := new int[1, 2];
    }
    """;
    CanParseAndPrintFeature(sourceStr);
  }

  [TestMethod]
  public void CanParseAndPrintIfStmt() {
    var sourceStr = """
    method Sign(x: int) returns (s: int)
    {
      if (x < 0) {
        s := -1;
      } else if (0 < x) {
        s := 1;
      } else {
        s := 0;
      }
    }
    """;
    CanParseAndPrintFeature(sourceStr);
  }

  [TestMethod]
  public void CanParseAndPrintReturnStmt() {
    var sourceStr = """
    method Identity(x: int) returns (y: int)
    {
      return x;
    }
    """;
    CanParseAndPrintFeature(sourceStr);
  }

  [TestMethod]
  public void CanParseAndPrintVarDeclStmt() {
    var sourceStr = """
    method Inc(x: int) returns (y: int)
    {
      var a, b: int := x, 1;
      return a + b;
    }
    """;
    CanParseAndPrintFeature(sourceStr);
  }

  [TestMethod]
  public void CanParseAndPrintMethodSpecifications() {
    var sourceStr = """
    method Double(x: int) returns (r: int)
      requires 0 <= x
      ensures r >= 2 * x
    {
      r := x + x;
    }
    """;
    CanParseAndPrintFeature(sourceStr);
  }

  [TestMethod]
  public void CanParseAndPrintMethodCalls() {
    var sourceStr = """
    method Foo(n: nat)
    {
    }
    
    method Bar()
    {
      Foo(1);
    }
    """;
    CanParseAndPrintFeature(sourceStr);
  }

  [TestMethod]
  public void CanParseAndPrintDefaultArguments() {
    var sourceStr = """
    method Foo(w: int, x: int, y: int := 0, z: int := 0)
    {
    }

    method Bar()
    {
      Foo(0, y := 1, x := 1);
    }
    """;
    CanParseAndPrintFeature(sourceStr);
  }

  [TestMethod]
  public void CanParseAndPrintITEExpr() {
    var sourceStr = """
    function Fib(n: nat): nat
    {
      if n < 2 then n else Fib(n - 2) + Fib(n - 1)
    }
    """;
    CanParseAndPrintFeature(sourceStr);
  }

  [TestMethod]
  public void CanParseAndPrintWhileLoop() {
    var sourceStr = """
    method Loop(n: nat)
    {
      var x := 0;
      while x < n
        invariant x <= n
      {
        x := x + 1;
      }
    }
    """;
    CanParseAndPrintFeature(sourceStr);
  }

  [TestMethod]
  public void CanParseAndPrintForLoop() {
    // TODO: handle cases e.g. `for i := ...` and `for _ := ...`
    var sourceStr = """
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
    CanParseAndPrintFeature(sourceStr);
  }

  [TestMethod]
  public void CanParseAndPrintChainingExpr() {
    var sourceStr = """
    function LteTen(n: nat): bool
    {
      0 <= n <= 10
    }
    """;
    CanParseAndPrintFeature(sourceStr);
  }

  [TestMethod]
  public void CanParseAndPrintSeqSelectExprForArray() {
    var sourceStr = """
    method Foo(a: array<int>, i: int)
    {
      var x1 := a[..];
      var x2 := a[i..];
      var x3 := a[..i];
      var x4 := a[0..i];
      var x5 := a[i];
    }
    """;
    CanParseAndPrintFeature(sourceStr);
  }
  
}