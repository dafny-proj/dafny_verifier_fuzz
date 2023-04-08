using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dafny = Microsoft.Dafny;
using DafnyW = DafnyWrappers.DafnyWrappers;

namespace AST.Tests;

[TestClass]
public class ASTTests
{
  [TestMethod]
  public void DafnyBacktranslation()
  {
    var prog = """
               method Sum(x: int, y: int)
                 returns (z: int)
               {
                 z := x + y;
               }
               """;
    Dafny.Program progDafny;
    DafnyW.ParseDafnyProgram(prog, out progDafny);
    var ast = Program.FromDafnyAST(progDafny);
    var backtranslated = Program.ToDafnyAST(ast);
    Assert.AreEqual(prog, DafnyW.DafnyProgramToString(backtranslated), /*ignoreCase=*/false);
  }

}