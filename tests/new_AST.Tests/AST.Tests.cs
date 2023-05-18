using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;
using DafnyW = DafnyWrappers.DafnyWrappers;

namespace AST_new.Tests;

using DafnyASTTranslator = Translation.DafnyASTTranslator;
using ASTPrinter = Printer.ASTPrinter;

[TestClass]
public class ASTTests {
  private void CanParseAndPrint(string sourceStr) {
    // Filter out '\r' from the string literals which messes up string 
    // comparison. It's weird that '\r' only started appearing in this file.
    sourceStr = Regex.Replace(sourceStr, "\r", "");
    var programDafny = DafnyW.ParseDafnyProgramFromString(sourceStr);
    DafnyW.ResolveDafnyProgram(programDafny);
    var program = DafnyASTTranslator.TranslateDafnyProgram(programDafny);
    var outputStr = ASTPrinter.NodeToString(program);
    Assert.AreEqual(sourceStr, outputStr.TrimEnd(), /*ignore_case=*/false);
  }

  [TestMethod]
  public void HelloWorld() {
    var sourceStr = """
    method Main()
    {
      print "Hello World!";
    }
    """;
    CanParseAndPrint(sourceStr);
  }
}