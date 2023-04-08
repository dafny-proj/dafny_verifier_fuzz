using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dafny = Microsoft.Dafny;

namespace DafnyWrappers.Tests;
using DafnyW = DafnyWrappers;

[TestClass]
public class DafnyWrappersTest
{
  [TestMethod]
  public void ParseAndPrint()
  {
    var programFile = "../../../../../examples/sum.dfy";
    var expected = $"// sum.dfy\n\n" + File.ReadAllText(programFile);

    Dafny.Program programDafny;
    DafnyW.ParseDafnyProgram(programFile, out programDafny);
    var actual = DafnyW.DafnyProgramToString(programDafny);

    Assert.AreEqual(expected, actual, /*ignoreCase=*/false);
  }
}