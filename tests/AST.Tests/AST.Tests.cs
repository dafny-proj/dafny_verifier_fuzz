using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DafnyW = DafnyWrappers.DafnyWrappers;

namespace AST.Tests;

[TestClass]
public class ASTTests {
  [TestMethod]
  public void SimpleParseAndPrint() {
    var sourceStr = """
    method Sum(x: int, y: int) returns (z: int)
    {
      z := x + y;
    }
    """;

    var programDafny = DafnyW.ParseDafnyProgramFromString(sourceStr);
    var program = Program.FromDafny(programDafny);
    var outputStr = Printer.ProgramToString(program);

    Assert.AreEqual(sourceStr, outputStr.TrimEnd(), /*ignore_case=*/false);
  }

  [TestMethod]
  public void SimpleMutation() {
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

    var mutFinder = new AST.Mutator.SimpleMutationFinder();
    mutFinder.VisitProgram(program);
    var muts = mutFinder.Mutations;
    Assert.AreEqual(1, muts.Count);

    mutFinder.Mutations.ElementAt(0).Apply();
    var mutant = Printer.ProgramToString(program);
    Assert.AreEqual(expected, mutant.TrimEnd(), /*ignore_case=*/false);
  }

}