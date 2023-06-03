namespace Fuzzer.Tests;

[TestClass]
public class ExprExtractionTest {
  [TestMethod]
  public void Test1() {
    var input = """
    function F(): int {
      0
    }
    """;
    var output = """
    function F(): int {
      fn1_mock(0)
    }

    function fn1_mock(fl0_mock: int): int {
      fl0_mock
    }
    """;
    var program = DafnyW.ParseProgramFromString(input);
    var mutator = new ExprExtractionMutator(
      new MockRandomizer(), new MockGenerator());
    var allExprs = ExprInfoBuilder.FindExprInfo(program);
    Assert.AreEqual(1, allExprs.Count);

    var e = allExprs[0];
    var mutation = new ExprExtractionMutation(
      exprToExtract: e,
      exprForParams: new List<ExprInfo>() { e },
      functionInjectionPoint: e.EnclosingModule.GetOrCreateDefaultClass()
    );
    mutator.ApplyMutation(mutation);
    var mutant = ASTPrinter.PrintNodeToString(program).TrimEnd();
    Assert.AreEqual(output, mutant);
  }
}
