using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DafnyW = DafnyWrappers.DafnyWrappers;

namespace AST.Tests;

[TestClass]
public class ASTTests {
  [TestMethod]
  public void SimpleParseAndPrint() {
    var filepath = "../../../../../examples/sum.dfy";
    var expected = File.ReadAllText(filepath);

    DafnyW.ParseDafnyProgram(filepath, out var programDafny);
    var program = Program.FromDafny(programDafny);
    var programStr = Printer.ProgramToString(program);

    Assert.AreEqual(expected, programStr, /*ignore_case=*/false);
  }

  [TestMethod]
  public void SimpleMutation() {
    var filepath = "../../../../../examples/sum.dfy";
    DafnyW.ParseDafnyProgram(filepath, out var programDafny);
    var program = Program.FromDafny(programDafny);
    var expected = """
                  method Sum(x: int, y: int) returns (z: int)
                  {
                    z := x - y;
                  }
                  """ + "\n";

    var mutFinder = new AST.Mutator.SimpleMutationFinder();
    mutFinder.VisitProgram(program);
    var muts = mutFinder.Mutations;
    Assert.AreEqual(1, muts.Count);

    mutFinder.Mutations.ElementAt(0).Apply();
    var mutant = Printer.ProgramToString(program);
    Assert.AreEqual(expected, mutant, /*ignore_case=*/false);
  }

}