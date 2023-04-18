using System.Linq;
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
  public void OperatorReplacementMutationFinder() {
    var sourceStr = """
    method Sum(x: int, y: int) returns (z: int)
    {
      z := x + y + y;
    }
    """;
    var programDafny = DafnyW.ParseDafnyProgramFromString(sourceStr);
    var program = Program.FromDafny(programDafny);

    var mutFinder = new AST.Mutator.OperatorReplacementMutationFinder();
    mutFinder.VisitProgram(program);
    var muts = mutFinder.Mutations;
    Assert.AreEqual(2, muts.Count);
  }

  [TestMethod]
  public void OperatorReplacementMutation() {
    var sourceStr = """
    method Sum(x: int, y: int) returns (z: int)
    {
      z := x + y;
    }
    """;
    var programDafny = DafnyW.ParseDafnyProgramFromString(sourceStr);
    var program = Program.FromDafny(programDafny);
    var expected = """
    method Sum(x: int, y: int) returns (z: int)
    {
      z := x - y;
    }
    """;

    var mutFinder = new AST.Mutator.OperatorReplacementMutationFinder();
    mutFinder.VisitProgram(program);
    var muts = mutFinder.Mutations;
    Assert.AreEqual(1, muts.Count);

    mutFinder.Mutations.ElementAt(0).Apply();
    var mutant = Printer.ProgramToString(program);
    Assert.AreEqual(expected, mutant.TrimEnd(), /*ignore_case=*/false);
  }

}