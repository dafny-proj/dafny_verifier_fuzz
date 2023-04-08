using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    var dafnyProg = DafnyW.ParseDafnyProgram(prog);
    var ast = Program.FromDafnyAST(dafnyProg);
    var backtranslated = Program.ToDafnyAST(ast);
    Assert.AreEqual(prog, DafnyW.DafnyProgramToString(backtranslated), /*ignoreCase=*/false);
  }

}