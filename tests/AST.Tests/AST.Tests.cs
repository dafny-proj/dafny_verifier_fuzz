using System.Text.RegularExpressions;

namespace AST.Tests;

[TestClass]
public class ASTTests {
  private void CanParseClonePrint(string sourceStr) {
    // Filter out '\r' from the string literals which messes up string 
    // comparison. It's weird that '\r' only started appearing in this file.
    sourceStr = Regex.Replace(sourceStr, "\r", "");
    var programDafny = DafnyW.ParseDafnyProgramFromString(sourceStr);
    DafnyW.ResolveDafnyProgram(programDafny);
    // Console.WriteLine(DafnyW.DafnyProgramToString(programDafny, postResolution: true));
    var program = ASTTranslator.TranslateDafnyProgram(programDafny);
    var clone = ASTCloner.Clone<Program>(program);
    var outputStr = ASTPrinter.PrintNodeToString(clone);
    Assert.AreEqual(sourceStr, outputStr.TrimEnd(), /*ignore_case=*/false);
  }

  [TestMethod]
  public void HelloWorld() {
    var sourceStr = """
    method Main() {
      print "Hello World!";
    }
    """;
    CanParseClonePrint(sourceStr);
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
    CanParseClonePrint(sourceStr);
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
    CanParseClonePrint(sourceStr);
  }

  [TestMethod]
  public void Array() {
    var sourceStr = """
    method ArrayInit() {
      var a1 := new int[3];
      var a2 := new int[3]((i) => i);
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
    CanParseClonePrint(sourceStr);
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
    CanParseClonePrint(sourceStr);
  }

  [TestMethod]
  public void Set() {
    var sourceStr = """
    method Set() {
      var s := {1, 2, 3};
    }
    """;
    CanParseClonePrint(sourceStr);
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
    CanParseClonePrint(sourceStr);
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
    CanParseClonePrint(mapSelect);
  }

  [TestMethod]
  public void VarDeclStmt() {
    var sourceStr = """
    method Main() {
      var i := 0;
    }
    """;
    CanParseClonePrint(sourceStr);
  }

  [TestMethod]
  public void AssignStmt() {
    var sourceStr = """
    method Identity(x: int) returns (y: int) {
      y := x;
    }
    """;
    CanParseClonePrint(sourceStr);
  }

  [TestMethod]
  public void ReturnStmt() {
    var sourceStr = """
    method Identity(x: int) returns (y: int) {
      return x;
    }

    method DoNothing() {
      return;
    }
    """;
    CanParseClonePrint(sourceStr);
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
    CanParseClonePrint(sourceStr);
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
    CanParseClonePrint(sourceStr);
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
    CanParseClonePrint(sourceStr);
  }

  [TestMethod]
  public void ITEExpr() {
    var sourceStr = """
    function IsNegative(x: int): bool {
      if x < 0 then true else false
    }
    """;
    CanParseClonePrint(sourceStr);
  }

  [TestMethod]
  public void LetExpr() {
    var sourceStr = """
    function Triple(x: int): int {
      var single, double := x, x + x; var triple := single + double; triple
    }
    """;
    CanParseClonePrint(sourceStr);
  }

  [TestMethod]
  public void Tuple() {
    var sourceStr = """
    method Tuple() {
      var x: (int, bool) := (1, true);
      var y := ();
      print x.0;
    }
    """;
    CanParseClonePrint(sourceStr);
  }

  [TestMethod]
  public void LiteralExpr() {
    var sourceStr = """
    class C {
    }

    method Literals() {
      var t, f := true, false;
      var c: char := '\n';
      var i: int := 12345;
      var r: real := 0.5;
      var s: string := "hello";
      var c0: C?, c1: C := null, new C;
    }
    """;
    CanParseClonePrint(sourceStr);
  }

  [TestMethod]
  public void QuantifierExpr() {
    var sourceStr = """
    function Forall(): bool {
      forall x: nat | x <= 5 :: x * x <= 25
    }

    function Exists(): bool {
      exists x: nat, y: nat | x <= 5 && y <= 5 :: x * y == 25
    }
    """;
    CanParseClonePrint(sourceStr);
  }

  [TestMethod]
  public void ForallStmt() {
    // TODO: Replace range with chaining expression when implemented.
    var forallAssign = """
    method ForallStmtAssign() {
      var a := new int[5];
      forall i: int | 0 <= i && i < a.Length {
        a[i] := i;
      }
    }
    """;
    var forallCall = """
    lemma P(i: int)

    method ForallStmtCall() {
      forall i: int | 0 <= i && i < 5 {
        P(i);
      }
    }
    """;
    var forallProof = """
    method ForallStmtProof() {
      forall i: int | 0 <= i && i < 5
        ensures i < 5
      {
      }
    }
    """;
    CanParseClonePrint(forallAssign);
    CanParseClonePrint(forallCall);
    CanParseClonePrint(forallProof);
  }

  [TestMethod]
  public void MatchExpr() {
    var sourceStr = """
    datatype Literal = Null | Bool(b: bool) | Int(i: int) | String(s: string)

    function ToInt(l: Literal): int {
      match l {
        case Null | Bool(false) => 0
        case Int(i) => i
        case _ => 1
      }
    }
    """;
    CanParseClonePrint(sourceStr);
  }

  [TestMethod]
  public void MatchStmt() {
    var sourceStr = """
    datatype OptionalInt = None | Some(i: int)

    method TryPrint(i: OptionalInt) {
      match i {
        case Some(j) => {
          print j;
        }
        case _ => 
      }
    }
    """;
    CanParseClonePrint(sourceStr);
  }

  [TestMethod]
  public void MultiSetFormingExpr() {
    var sourceStr = """
    method M(s: set<int>) {
      var ms := multiset(s);
    }
    """;
    CanParseClonePrint(sourceStr);
  }

  [TestMethod]
  public void ArrayLength() {
    var sourceStr = """
    method ArrayLength(a: array<int>) {
      print a.Length;
    }
    """;
    CanParseClonePrint(sourceStr);
  }

  [TestMethod]
  public void ArrowType() {
    // Syntax such as `int -> int` is accepted but for simplicity, we always 
    // choose to print the arguments in parentheses.
    var sourceStr = """
    function F0(f: () -> bool): bool {
      f()
    }

    function F1(f: (int) -> bool): bool {
      f(1)
    }

    function F2(f: (int, int) -> bool): bool {
      f(1, 1)
    }

    function F3(f: ((int, int)) -> bool): bool {
      f((1, 1))
    }
    
    function F4(f: ((int) -> bool) -> bool): bool

    function F5(f: (int) -> (bool) -> bool): bool
    """;
    CanParseClonePrint(sourceStr);
  }

  [TestMethod]
  public void LambdaExpr() {
    var sourceStr = """
    function Equals(): (int, int) -> bool {
      (x, y) => x == y
    }
    """;
    CanParseClonePrint(sourceStr);
  }

  [TestMethod]
  public void DatatypeValueUpdate() {
    var sourceStr = """
    datatype Coordinate = Dim3(x: int, y: int, z: int)

    function ProjectDim1(c: Coordinate): Coordinate {
      c.(y := 0, z := 0)
    }
    """;
    CanParseClonePrint(sourceStr);
  }

  [TestMethod]
  public void SetComprehension() {
    // TODO: Replace range with chaining expression when implemented.
    var sourceStr = """
    method M() {
      var s1 := set x: nat, y: nat | x < y && y < 100 :: x * y;
      var s2 := set x: nat | x in s1;
    }
    """;
    CanParseClonePrint(sourceStr);
  }

  [TestMethod]
  public void MapComprehension() {
    // TODO: Replace range with chaining expression when implemented.
    var sourceStr = """
    method M() {
      var m1 := map x: int | 0 <= x && x <= 10 :: x * x;
      var m2 := map x: int | 0 <= x && x <= 10 :: x := x * x;
    }
    """;
    CanParseClonePrint(sourceStr);
  }

  [TestMethod]
  public void SeqConstruction() {
    var sourceStr = """
    method M() {
      var s := seq(3, (i) => i);
    }
    """;
    CanParseClonePrint(sourceStr);
  }

  [TestMethod]
  public void TypeUnaryExpr() {
    var sourceStr = """
    type Int = int

    function F(a: Int): int {
      if a is int then a as int else 0
    }
    """;
    CanParseClonePrint(sourceStr);
  }

  [TestMethod]
  public void WildVar() {
    var sourceStr = """
    method M() {
      var _ := 1;
    }
    """;
    CanParseClonePrint(sourceStr);
  }

  [TestMethod]
  public void GenericMethod() {
    var sourceStr = """
    method M<T>(t: T)
    """;
    CanParseClonePrint(sourceStr);
  }

  [TestMethod]
  public void GenericFunction() {
    var sourceStr = """
    function F<T>(t: T): T
    """;
    CanParseClonePrint(sourceStr);
  }

  [TestMethod]
  public void GenericClass() {
    var sourceStr = """
    class GC<T> {
      var t: T
      constructor(t_: T) {
        t := t_;
      }
    }

    method M() {
      var gc: GC<bool> := new GC<bool>(true);
    }
    """;
    CanParseClonePrint(sourceStr);
  }

}