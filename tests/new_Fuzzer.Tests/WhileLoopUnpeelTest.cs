namespace Fuzzer.Tests;

[TestClass]
public class WhileLoopUnpeelTest {
  public void TestWhileLoopUnpeel(
    string input,
    string expectedOutput,
    int expectedNumMutationsFound = 1,
    int mutationToTrigger = 0
  ) {
    var program = DafnyW.ParseProgramFromString(input);
    var mutator = new WhileLoopUnpeelMutator(
      new MockRandomizer(), new MockGenerator());
    var potentialMutations = mutator.FindPotentialMutations(program);
    Assert.AreEqual(expectedNumMutationsFound, potentialMutations.Count);

    var mutation = potentialMutations[mutationToTrigger];
    mutator.ApplyMutation(mutation);
    var mutant = ASTPrinter.NodeToString(program).TrimEnd();
    Assert.AreEqual(expectedOutput, mutant);
  }

  [TestMethod]
  public void UnrollSimpleWhileLoop() {
    var whileWhole = """
    method Foo(n: nat) {
      var i := 0;
      while i < n {
        i := i + 1;
      }
    }
    """;
    var whileUnrolled = """
    method Foo(n: nat) {
      var i := 0;
      if i < n {
        i := i + 1;
      }
      while i < n {
        i := i + 1;
      }
    }
    """;
    TestWhileLoopUnpeel(whileWhole, whileUnrolled);
  }

  [TestMethod]
  public void UnrollWhileLoopWithUnconditionalBreak() {
    var whileWhole = """
    method Foo(n: nat) {
      print 0;
      while * {
        print 1;
        break;
        print 2;
      }
      print 3;
    }
    """;
    var whileUnrolled = """
    method Foo(n: nat) {
      print 0;
      var v0_mock: bool := false;
      label l1_mock:
      if * {
        print 1;
        v0_mock := true;
        break l1_mock;
        print 2;
      }
      if !v0_mock {
        while * {
          print 1;
          break;
          print 2;
        }
      }
      print 3;
    }
    """;
    TestWhileLoopUnpeel(whileWhole, whileUnrolled);
  }

  [TestMethod]
  public void UnrollWhileLoopWithConditionalBreak() {
    var whileWhole = """
    function Bool(n: nat): bool

    method Foo(n: nat) {
      print 0;
      while Bool(0) {
        print 1;
        if Bool(1) {
          print 2;
          break;
          print 3;
        } else {
          print 4;
        }
        print 5;
      }
      print 6;
    }
    """;
    var whileUnrolled = """
    function Bool(n: nat): bool

    method Foo(n: nat) {
      print 0;
      var v0_mock: bool := false;
      label l1_mock:
      if Bool(0) {
        print 1;
        if Bool(1) {
          print 2;
          v0_mock := true;
          break l1_mock;
          print 3;
        } else {
          print 4;
        }
        print 5;
      }
      if !v0_mock {
        while Bool(0) {
          print 1;
          if Bool(1) {
            print 2;
            break;
            print 3;
          } else {
            print 4;
          }
          print 5;
        }
      }
      print 6;
    }
    """;
    TestWhileLoopUnpeel(whileWhole, whileUnrolled);
  }

  [TestMethod]
  public void UnrollWhileLoopWithNestedConditionalBreak() {
    var whileWhole = """
    function Bool(n: nat): bool

    method Foo(n: nat) {
      print 0;
      while Bool(0) {
        print 1;
        if Bool(1) {
          print 2;
          if Bool(2) {
            print 3;
            break;
            print 4;
          }
          print 5;
        }
        print 6;
      }
      print 7;
    }
    """;
    var whileUnrolled = """
    function Bool(n: nat): bool

    method Foo(n: nat) {
      print 0;
      var v0_mock: bool := false;
      label l1_mock:
      if Bool(0) {
        print 1;
        if Bool(1) {
          print 2;
          if Bool(2) {
            print 3;
            v0_mock := true;
            break l1_mock;
            print 4;
          }
          print 5;
        }
        print 6;
      }
      if !v0_mock {
        while Bool(0) {
          print 1;
          if Bool(1) {
            print 2;
            if Bool(2) {
              print 3;
              break;
              print 4;
            }
            print 5;
          }
          print 6;
        }
      }
      print 7;
    }
    """;
    TestWhileLoopUnpeel(whileWhole, whileUnrolled);
  }

  [TestMethod]
  public void UnrollNestedWhileLoopWithInnerBreak() {
    var whileWhole = """
    function Bool(n: nat): bool

    method Foo(n: nat) {
      print 0;
      while Bool(0) {
        print 1;
        while Bool(1) {
          print 2;
          break;
          print 3;
        }
        print 4;
      }
      print 5;
    }
    """;
    var outerWhileUnrolled = """
    function Bool(n: nat): bool

    method Foo(n: nat) {
      print 0;
      if Bool(0) {
        print 1;
        while Bool(1) {
          print 2;
          break;
          print 3;
        }
        print 4;
      }
      while Bool(0) {
        print 1;
        while Bool(1) {
          print 2;
          break;
          print 3;
        }
        print 4;
      }
      print 5;
    }
    """;
    var innerWhileUnrolled = """
    function Bool(n: nat): bool

    method Foo(n: nat) {
      print 0;
      while Bool(0) {
        print 1;
        var v0_mock: bool := false;
        label l1_mock:
        if Bool(1) {
          print 2;
          v0_mock := true;
          break l1_mock;
          print 3;
        }
        if !v0_mock {
          while Bool(1) {
            print 2;
            break;
            print 3;
          }
        }
        print 4;
      }
      print 5;
    }
    """;
    TestWhileLoopUnpeel(whileWhole,
      outerWhileUnrolled,
      expectedNumMutationsFound: 2,
      mutationToTrigger: 0);
    TestWhileLoopUnpeel(whileWhole,
      innerWhileUnrolled,
      expectedNumMutationsFound: 2,
      mutationToTrigger: 1);
  }

  [TestMethod]
  public void UnrollNestedWhileLoopWithInnerBreakBreak() {
    var whileWhole = """
    function Bool(n: nat): bool

    method Foo(n: nat) {
      print 0;
      while Bool(0) {
        print 1;
        while Bool(1) {
          print 2;
          break break;
          print 3;
        }
        print 4;
      }
      print 5;
    }
    """;
    var outerWhileUnrolled = """
    function Bool(n: nat): bool

    method Foo(n: nat) {
      print 0;
      var v0_mock: bool := false;
      label l1_mock:
      if Bool(0) {
        print 1;
        while Bool(1) {
          print 2;
          v0_mock := true;
          break l1_mock;
          print 3;
        }
        print 4;
      }
      if !v0_mock {
        while Bool(0) {
          print 1;
          while Bool(1) {
            print 2;
            break break;
            print 3;
          }
          print 4;
        }
      }
      print 5;
    }
    """;
    var innerWhileUnrolled = """
    function Bool(n: nat): bool

    method Foo(n: nat) {
      print 0;
      while Bool(0) {
        print 1;
        if Bool(1) {
          print 2;
          break;
          print 3;
        }
        while Bool(1) {
          print 2;
          break break;
          print 3;
        }
        print 4;
      }
      print 5;
    }
    """;
    TestWhileLoopUnpeel(whileWhole,
      outerWhileUnrolled,
      expectedNumMutationsFound: 2,
      mutationToTrigger: 0);
    TestWhileLoopUnpeel(whileWhole,
      innerWhileUnrolled,
      expectedNumMutationsFound: 2,
      mutationToTrigger: 1);
  }
}
