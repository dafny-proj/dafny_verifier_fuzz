using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dafny = Microsoft.Dafny;
using DafnyW = DafnyWrappers.DafnyWrappers;

namespace AST.Tests;

[TestClass]
public class ASTTests
{
  [TestMethod]
  public void SimpleParseAndPrint() {
    var filepath = "../../../../../examples/sum.dfy";
    var expected = File.ReadAllText(filepath);

    DafnyW.ParseDafnyProgram(filepath, out var programDafny);
    var program = Program.FromDafny(programDafny);
    var programStr = Printer.ProgramToString(program);
    
    Assert.AreEqual(expected, programStr, /*ignore_case=*/false);
  }

}