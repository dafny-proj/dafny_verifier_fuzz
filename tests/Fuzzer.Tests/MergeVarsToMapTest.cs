namespace Fuzzer.Tests;

[TestClass]
public class MergeVarsToMapTest {
  public void TestMergeVarsToMap(
    string input,
    string? expectedOutput,
    int expectedNumMutationsFound = 1,
    int mutationToTrigger = 0,
    int maxVarsMerged = -1
  ) {
    var program = DafnyW.ParseProgramFromString(input);
    var mutator = new MergeVarsToMapMutator(
      new MockRandomizer(), new MockGenerator());
    var potentialMutations = mutator.FindPotentialMutations(program);
    Assert.AreEqual(expectedNumMutationsFound, potentialMutations.Count);

    if (expectedNumMutationsFound == 0 || expectedOutput == null) { return; }

    var mutation = potentialMutations[mutationToTrigger];
    if (maxVarsMerged != -1) {
      mutation.Vars = mutation.Vars.Take(
        Math.Min(mutation.Vars.Count, maxVarsMerged)).ToList();
    }
    mutator.ApplyMutation(mutation);
    var mutant = ASTPrinter.PrintNodeToString(program).TrimEnd();
    Assert.AreEqual(expectedOutput, mutant);
  }

  [TestMethod]
  public void Test1() {
    var source = """
    method M() {
      var a: int := 1;
      a := a + 1;
    }
    """;
    var target = """
    method M() {
      var v0_mock: map<string, int> := map[];
      v0_mock := v0_mock["a" := 1];
      v0_mock := v0_mock["a" := v0_mock["a"] + 1];
    }
    """;
    TestMergeVarsToMap(source, target);
  }

  [TestMethod]
  public void Test2() {
    var source = """
    method M() {
      var a, b := 1, 1;
    }
    """;
    var target = """
    method M() {
      var v0_mock: map<string, int> := map[];
      v0_mock := v0_mock + map["a" := 1, "b" := 1];
    }
    """;
    TestMergeVarsToMap(source, target);
  }

  [TestMethod]
  public void Test3() {
    var source = """
    method M() {
      var a := 1;
      var b := 1;
    }
    """;
    var target = """
    method M() {
      var v0_mock: map<string, int> := map[];
      v0_mock := v0_mock["a" := 1];
      v0_mock := v0_mock["b" := 1];
    }
    """;
    TestMergeVarsToMap(source, target);
  }

  [TestMethod]
  public void Test4() {
    var source = """
    method M() {
      var a, b := 1, 1;
      a, b := b, a;
    }
    """;
    var target = """
    method M() {
      var v0_mock: map<string, int> := map[];
      v0_mock := v0_mock + map["a" := 1, "b" := 1];
      v0_mock := v0_mock + map["a" := v0_mock["b"], "b" := v0_mock["a"]];
    }
    """;
    TestMergeVarsToMap(source, target);
  }

  [TestMethod]
  public void Test5() {
    var source = """
    method M() {
      var a, b, c := 1, 1, true;
    }
    """;
    var target = """
    method M() {
      var v0_mock: map<string, int> := map[];
      var c := true;
      v0_mock := v0_mock + map["a" := 1, "b" := 1];
    }
    """;
    TestMergeVarsToMap(source, target,
      expectedNumMutationsFound: 2,
      mutationToTrigger: 0);
  }

  [TestMethod]
  public void Test6() {
    var source = """
    class C {
    }

    method M(c0: C, c1: C) {
      var a, b := c0, c1;
    }
    """;
    var target = """
    class C {
    }

    method M(c0: C, c1: C) {
      var v0_mock: map<string, C> := map[];
      v0_mock := v0_mock + map["a" := c0, "b" := c1];
    }
    """;
    TestMergeVarsToMap(source, target);
  }

  [TestMethod]
  public void Test7() {
    var source = """
    class C {
    }

    method M() {
      var a := new C;
    }
    """;
    TestMergeVarsToMap(source, null, expectedNumMutationsFound: 0);
  }

  [TestMethod]
  public void Test8() {
    var source = """
    method M0() returns (x: int)

    method M() {
      var a := M0();
    }
    """;
    TestMergeVarsToMap(source, null, expectedNumMutationsFound: 0);
  }

  [TestMethod]
  public void Test9() {
    var source = """
    method M(b: bool) {
      var x := 1;
      if b {
        var y := x;
      } else {
        var y := x;
      }
      var z := 1;
    }
    """;
    var target = """
    method M(b: bool) {
      var v0_mock: map<string, int> := map[];
      v0_mock := v0_mock["x" := 1];
      if b {
        var y := v0_mock["x"];
      } else {
        var y := v0_mock["x"];
      }
      v0_mock := v0_mock["z" := 1];
    }
    """;
    TestMergeVarsToMap(source, target,
      expectedNumMutationsFound: 3,
      mutationToTrigger: 0);
  }

  [TestMethod]
  public void Test10() {
    var source = """
    method M(b: bool) {
      var x := 1;
      if b {
        var y := x;
      } else {
        var y := x;
      }
      var z := 1;
    }
    """;
    var target = """
    method M(b: bool) {
      var x := 1;
      if b {
        var v0_mock: map<string, int> := map[];
        v0_mock := v0_mock["y" := x];
      } else {
        var y := x;
      }
      var z := 1;
    }
    """;
    TestMergeVarsToMap(source, target,
      expectedNumMutationsFound: 3,
      mutationToTrigger: 1);
  }

}

