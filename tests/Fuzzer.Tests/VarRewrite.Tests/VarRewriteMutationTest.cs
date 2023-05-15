namespace Fuzzer.Tests;

[TestClass]
public class VarRewriteMutationTest {

  [TestMethod]
  public void SingleVarToMap() {
    var aVar = """
    method M()
    {
      var a: int := 1;
      a := a + 1;
    }
    """;
    var aMap = """
    method M()
    {
      var m: map<string, int> := map[];
      m := m["a" := 1];
      m := m["a" := m["a"] + 1];
    }
    """;
    var programDafny = DafnyW.ParseDafnyProgramFromString(aVar);
    DafnyW.ResolveDafnyProgram(programDafny);
    var program = Program.FromDafny(programDafny);

    var scopeBuilder = new ScopeBuilder(program);
    scopeBuilder.Build();
    Assert.AreEqual(2, scopeBuilder.Scopes.Count);

    var targetScope = scopeBuilder.Scopes[1];
    var varsToMerge = targetScope.Vars.Values.ToList();
    var rewriter = new VarMapRewriter(varsToMerge, targetScope.Node);
    rewriter.Rewrite();

    var mutant = Printer.ProgramToString(program);
    var expected = aMap;
    Assert.AreEqual(expected, mutant.TrimEnd());
  }

    [TestMethod]
  public void MultipleVarsToMap() {
    var aVar = """
    method M()
    {
      var a: int, b: int := 1, 2;
      a, b := b, a;
    }
    """;
    var aMap = """
    method M()
    {
      var m: map<string, int> := map[];
      m := m + map["a" := 1, "b" := 2];
      m := m + map["a" := m["b"], "b" := m["a"]];
    }
    """;
    var programDafny = DafnyW.ParseDafnyProgramFromString(aVar);
    DafnyW.ResolveDafnyProgram(programDafny);
    var program = Program.FromDafny(programDafny);

    var scopeBuilder = new ScopeBuilder(program);
    scopeBuilder.Build();
    Assert.AreEqual(2, scopeBuilder.Scopes.Count);

    var targetScope = scopeBuilder.Scopes[1];
    var varsToMerge = targetScope.Vars.Values.ToList();
    var rewriter = new VarMapRewriter(varsToMerge, targetScope.Node);
    rewriter.Rewrite();

    var mutant = Printer.ProgramToString(program);
    var expected = aMap;
    Assert.AreEqual(expected, mutant.TrimEnd());
  }

  [TestMethod]
  public void SingleVarToClass() {
    var aVar = """
    method M()
    {
      var a: int := 1;
      a := a + 1;
    }
    """;
    var aMap = """
    class C {
      var a: int;

      constructor()
      {
      }
    }

    method M()
    {
      var c: C := new C();
      c.a := 1;
      c.a := c.a + 1;
    }
    """;
    var programDafny = DafnyW.ParseDafnyProgramFromString(aVar);
    DafnyW.ResolveDafnyProgram(programDafny);
    var program = Program.FromDafny(programDafny);

    var scopeBuilder = new ScopeBuilder(program);
    scopeBuilder.Build();
    Assert.AreEqual(2, scopeBuilder.Scopes.Count);

    var targetScope = scopeBuilder.Scopes[1];
    var varsToMerge = targetScope.Vars.Values.ToList();
    var rewriter = new VarClassRewriter(varsToMerge, targetScope.Node, program);
    rewriter.Rewrite();

    var mutant = Printer.ProgramToString(program);
    var expected = aMap;
    Assert.AreEqual(expected, mutant.TrimEnd());
  }
}