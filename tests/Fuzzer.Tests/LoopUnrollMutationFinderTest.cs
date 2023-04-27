namespace Fuzzer.Tests;

[TestClass]
public class LoopUnrollMutationFinderTest {
  [TestMethod]
  public void FindsWhileLoop() {
    var finder = new LoopUnrollMutationFinder();
    var sourceStr = """
    method Foo() {
      while (true) {}
    }
    """;
    var programDafny = DafnyW.ParseDafnyProgramFromString(sourceStr);
    DafnyW.ResolveDafnyProgram(programDafny);
    var program = Program.FromDafny(programDafny);
    finder.FindMutations(program);
    Assert.AreEqual(1, finder.NumMutationsFound);   
  }
}