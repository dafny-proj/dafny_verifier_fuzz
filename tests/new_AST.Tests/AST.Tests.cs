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
    method Main() {
      print "Hello World!";
    }
    """;
    CanParseAndPrint(sourceStr);
  }

  [TestMethod]
  public void Classes() {
    var sourceStr = """
    class C {
      var f1: int
      constructor() {
        f1 := 1;
      }

      constructor WithArguments(f1: int) {
        this.f1 := f1;
      }

      method SetF1(f1: int)
        modifies this
      {
        this.f1 := f1;
      }
    }

    method Main() {
      var c1 := new C();
      var c2 := new C.WithArguments(1);
      assert c1.f1 == c2.f1;
      c1.SetF1(0);
    }
    """;
    CanParseAndPrint(sourceStr);
  }

  [TestMethod]
  public void Datatypes() {
    var sourceStr = """
    datatype Optional<T> = None | Some(value: T) {
      function GetValue(): T
        requires this.Some?
      {
        value
      }
    }

    method GetValue<K, V>(m: map<K, V>, i: K) returns (v: Optional<V>) {
      v := Optional.None;
      if (i in m) {
        v := Optional.Some(m[i]);
      }
    }
    """;
    CanParseAndPrint(sourceStr);
  }

  [TestMethod]
  public void Array() {
    var sourceStr = """
    method ArrayInit() {
      var a1 := new int[3];
      var a3 := new int[3][1, 2, 3];
    }

    method ArrayIndexing(a: array<int>, i: int) {
      var x1 := a[..];
      var x2 := a[i..];
      var x3 := a[..i];
      var x4 := a[0..i];
      var x5 := a[i];
    }
    """;
    // var TODO = """
    // // Unsupported lambdas and arrow types.
    // var a2 := new int[3](i => i);
    // """;
    CanParseAndPrint(sourceStr);
  }

  [TestMethod]
  public void Seq() {
    var sourceStr = """
    method Seq() {
      var s, i := [1, 2, 3], 0;
      var x1 := s[..];
      var x2 := s[i..];
      var x3 := s[..i];
      var x4 := s[0..i];
      var x5 := s[i];
    }
    """;
    CanParseAndPrint(sourceStr);
  }

  [TestMethod]
  public void Set() {
    var sourceStr = """
    method Set() {
      var s := {1, 2, 3};
    }
    """;
    CanParseAndPrint(sourceStr);
  }

  [TestMethod]
  public void MultiSet() {
    var sourceStr = """
    method MultiSet() {
      var s: multiset<int> := multiset{};
      s := multiset{1, 2, 2, 3};
      s := s[3 := 3];
      assert s[1] == 1 && s[2] == 2 && s[3] == 3;
    }
    """;
    CanParseAndPrint(sourceStr);
  }

  [TestMethod]
  public void Map() {
    var mapSelect = """
    method Map() {
      var m: map<int, int> := map[];
      m := map[1 := 1, 2 := 2, 3 := 1];
      m := m[3 := 3];
      assert m[1] == 1 && m[2] == 2 && m[3] == 3;
    }
    """;
    CanParseAndPrint(mapSelect);
  }

  [TestMethod]
  public void VarDeclStmt() {
    var sourceStr = """
    method Main() {
      var i := 0;
    }
    """;
    CanParseAndPrint(sourceStr);
  }

  [TestMethod]
  public void AssignStmt() {
    var sourceStr = """
    method Identity(x: int) returns (y: int) {
      y := x;
    }
    """;
    CanParseAndPrint(sourceStr);
  }

  [TestMethod]
  public void ReturnStmt() {
    var sourceStr = """
    method Identity(x: int) returns (y: int) {
      return x;
    }
    """;
    CanParseAndPrint(sourceStr);
  }

  [TestMethod]
  public void IfStmt() {
    var sourceStr = """
    method Sign(x: int) returns (s: int) {
      if (x < 0) {
        s := -1;
      } else if (0 < x) {
        s := 1;
      } else {
        s := 0;
      }
    }
    """;
    CanParseAndPrint(sourceStr);
  }

  [TestMethod]
  public void WhileLoop() {
    var sourceStr = """
    method Loop(n: nat) {
      var x := 0;
      while x < n
        invariant x <= n
      {
        x := x + 1;
      }
    }
    """;
    CanParseAndPrint(sourceStr);
  }

  [TestMethod]
  public void ForLoop() {
    var sourceStr = """
    method Fibonacci(n: nat) returns (x: nat) {
      x := 0;
      var y := 1;
      for i := 0 to n {
        x, y := y, x + y;
      }
    }
    """;
    CanParseAndPrint(sourceStr);
  }

}