namespace Fuzzer.Tests;

[TestClass]
public class MergeVarsToClassTest {
  public void TestMergeVarsToClass(
    string input,
    string? expectedOutput,
    int expectedNumMutationsFound = 1,
    int mutationToTrigger = 0,
    int maxVarsMerged = -1
  ) {
    var program = DafnyW.ParseProgramFromString(input);
    var mutator = new MergeVarsToClassMutator(
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
    class C0_mock {
      var a: int
    }

    method M() {
      var v1_mock: C0_mock := new C0_mock;
      v1_mock.a := 1;
      v1_mock.a := v1_mock.a + 1;
    }
    """;
    TestMergeVarsToClass(source, target);
  }

  [TestMethod]
  public void Test2() {
    var source = """
    method M() {
      var a, b := 1, true;
    }
    """;
    var target = """
    class C0_mock {
      var a: int
      var b: bool
    }

    method M() {
      var v1_mock: C0_mock := new C0_mock;
      v1_mock.a, v1_mock.b := 1, true;
    }
    """;
    TestMergeVarsToClass(source, target);
  }

  [TestMethod]
  public void Test3() {
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
    class C0_mock {
      var x: int
      var z: int
    }

    method M(b: bool) {
      var v1_mock: C0_mock := new C0_mock;
      v1_mock.x := 1;
      if b {
        var y := v1_mock.x;
      } else {
        var y := v1_mock.x;
      }
      v1_mock.z := 1;
    }
    """;
    TestMergeVarsToClass(source, target,
      expectedNumMutationsFound: 3,
      mutationToTrigger: 0);
  }

  [TestMethod]
  public void Test4() {
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
    class C0_mock {
      var y: int
    }

    method M(b: bool) {
      var x := 1;
      if b {
        var v1_mock: C0_mock := new C0_mock;
        v1_mock.y := x;
      } else {
        var y := x;
      }
      var z := 1;
    }
    """;
    TestMergeVarsToClass(source, target,
      expectedNumMutationsFound: 3,
      mutationToTrigger: 1);
  }

  [TestMethod]
  public void Test5() {
    var source = """
    method M() returns (x: int) {
      var y := M();
    }
    """;
    var target = """
    class C0_mock {
      var y: int
    }

    method M() returns (x: int) {
      var v1_mock: C0_mock := new C0_mock;
      v1_mock.y := M();
    }
    """;
    TestMergeVarsToClass(source, target);
  }

}

