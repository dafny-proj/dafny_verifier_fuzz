namespace MutantGenerator.Tests;

[TestClass]
public class WhileLoopPeelTest {
  public void TestWhileLoopPeel(
    string input,
    string expectedOutput,
    int expectedNumMutationsFound = 1,
    int mutationToTrigger = 0
  ) {
    var program = DafnyW.ParseProgramFromString(input);
    var mutator = new WhileLoopPeelMutator(
      new MockRandomizer(), new MockGenerator());
    var potentialMutations = mutator.FindPotentialMutations(program);
    Assert.AreEqual(expectedNumMutationsFound, potentialMutations.Count);

    var mutation = potentialMutations[mutationToTrigger];
    mutator.ApplyMutation(mutation);
    var mutant = ASTPrinter.PrintNodeToString(program).TrimEnd();
    Assert.AreEqual(expectedOutput, mutant);
  }

  [TestMethod]
  public void PeelSimpleWhileLoop() {
    var whileWhole = """
    method Foo(n: nat) {
      var i := 0;
      while i < n {
        i := i + 1;
      }
    }
    """;
    var whilePeeled = """
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
    TestWhileLoopPeel(whileWhole, whilePeeled);
  }

  [TestMethod]
  public void PeelWhileLoopWithInvariants() {
    var whileWhole = """
    method Foo(n: nat) {
      var i := 0;
      while i < n 
        invariant 0 <= i
        invariant i <= n
      {
        i := i + 1;
      }
    }
    """;
    var whilePeeled = """
    method Foo(n: nat) {
      var i := 0;
      assert 0 <= i;
      assert i <= n;
      if i < n {
        i := i + 1;
      }
      while i < n
        invariant 0 <= i
        invariant i <= n
      {
        i := i + 1;
      }
    }
    """;
    TestWhileLoopPeel(whileWhole, whilePeeled);
  }

  [TestMethod]
  public void PeelWhileLoopWithUnconditionalBreak() {
    var whileWhole = """
    method Foo(n: nat) {
      print 0;
      while *
        invariant true
      {
        print 1;
        break;
        print 2;
      }
      print 3;
    }
    """;
    var whilePeeled = """
    method Foo(n: nat) {
      print 0;
      var v0_mock: bool := false;
      assert true;
      label l1_mock:
      if * {
        print 1;
        v0_mock := true;
        break l1_mock;
        print 2;
      }
      if !v0_mock {
        while *
          invariant true
        {
          print 1;
          break;
          print 2;
        }
      }
      print 3;
    }
    """;
    TestWhileLoopPeel(whileWhole, whilePeeled);
  }

  [TestMethod]
  public void PeelWhileLoopWithConditionalBreak() {
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
    var whilePeeled = """
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
    TestWhileLoopPeel(whileWhole, whilePeeled);
  }

  [TestMethod]
  public void PeelWhileLoopWithNestedConditionalBreak() {
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
    var whilePeeled = """
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
    TestWhileLoopPeel(whileWhole, whilePeeled);
  }

  [TestMethod]
  public void PeelNestedWhileLoopWithInnerBreak() {
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
    var outerWhilePeeled = """
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
    var innerWhilePeeled = """
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
    TestWhileLoopPeel(whileWhole,
      outerWhilePeeled,
      expectedNumMutationsFound: 2,
      mutationToTrigger: 0);
    TestWhileLoopPeel(whileWhole,
      innerWhilePeeled,
      expectedNumMutationsFound: 2,
      mutationToTrigger: 1);
  }

  [TestMethod]
  public void PeelNestedWhileLoopWithInnerBreakBreak() {
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
    var outerWhilePeeled = """
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
    var innerWhilePeeled = """
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
    TestWhileLoopPeel(whileWhole,
      outerWhilePeeled,
      expectedNumMutationsFound: 2,
      mutationToTrigger: 0);
    TestWhileLoopPeel(whileWhole,
      innerWhilePeeled,
      expectedNumMutationsFound: 2,
      mutationToTrigger: 1);
  }

  [TestMethod]
  public void PeelWhileLoopWithContinue() {
    var whileWhole = """
    method Foo(n: nat) {
      print 0;
      while * {
        print 1;
        continue;
        print 2;
      }
      print 3;
    }
    """;
    var whilePeeled = """
    method Foo(n: nat) {
      print 0;
      label l0_mock:
      if * {
        print 1;
        break l0_mock;
        print 2;
      }
      while * {
        print 1;
        continue;
        print 2;
      }
      print 3;
    }
    """;
    TestWhileLoopPeel(whileWhole, whilePeeled);
  }

}
