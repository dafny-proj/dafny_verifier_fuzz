using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dafny = Microsoft.Dafny;

namespace DafnyWrappers.Tests;
using DafnyW = DafnyWrappers;

[TestClass]
public class DafnyWrappersTest {
  [TestMethod]
  public void ParseAndPrintFromFile() {
    var programFile = "../../../../../examples/sum.dfy";
    var expected = $"// sum.dfy\n\n" + File.ReadAllText(programFile);

    var programDafny = DafnyW.ParseDafnyProgramFromFile(programFile);
    var actual = DafnyW.DafnyProgramToString(programDafny);

    Assert.AreEqual(expected, actual, /*ignoreCase=*/false);
  }
}