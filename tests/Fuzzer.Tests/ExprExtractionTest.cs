namespace Fuzzer.Tests;

public class ExprExtractionTestRandomizer : Randomizer {
  public List<bool>? Bools { get; set; }
  private int randBoolCount = 0;
  public override bool RandBool() => Bools?[randBoolCount++] ?? base.RandBool();
}

[TestClass]
public class ExprExtractionTest {
  private void TestExprExtraction(string input, string output,
  int expectedNumExprsFound = 1,
  int exprToExtractIndex = 0,
  List<bool>? randomizerChoices = null) {
    var program = DafnyW.ParseProgramFromString(input);
    var randomizer = new ExprExtractionTestRandomizer();
    randomizer.Bools = randomizerChoices;
    var mutator = new ExprExtractionMutator(randomizer, new MockGenerator());
    var allExprs = ExprInfoBuilder.FindExprInfo(program);
    Assert.AreEqual(expectedNumExprsFound, allExprs.Count);

    var e = allExprs.ElementAt(exprToExtractIndex).Value;
    var mutation = new ExprExtractionMutation(exprToExtract: e);
    mutator.ApplyMutation(mutation);
    var mutant = ASTPrinter.PrintNodeToString(program).TrimEnd();
    Assert.AreEqual(output, mutant);
  }

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
    TestExprExtraction(input, output,
      expectedNumExprsFound: 1,
      exprToExtractIndex: 0,
      randomizerChoices: new() { true });
  }

  [TestMethod]
  public void Test2() {
    var input = """
    function F(): int {
      0
    }
    """;
    var output = """
    function F(): int {
      fn0_mock()
    }

    function fn0_mock(): int {
      0
    }
    """;
    TestExprExtraction(input, output,
       expectedNumExprsFound: 1,
       exprToExtractIndex: 0,
       randomizerChoices: new() { false });
  }

  [TestMethod]
  public void Test3() {
    var input = """
    function F(x: int, y: int): int {
      x + y
    }
    """;
    var output = """
    function F(x: int, y: int): int {
      fn2_mock(x, y)
    }

    function fn2_mock(fl0_mock: int, fl1_mock: int): int {
      fl0_mock + fl1_mock
    }
    """;
    TestExprExtraction(input, output,
      expectedNumExprsFound: 3,
      exprToExtractIndex: 0,
      randomizerChoices: new() { false });
  }

  [TestMethod]
  public void Test4() {
    var input = """
    function F(x: int, y: int): int {
      x + y
    }
    """;
    var output = """
    function F(x: int, y: int): int {
      fn1_mock(x + y)
    }

    function fn1_mock(fl0_mock: int): int {
      fl0_mock
    }
    """;
    TestExprExtraction(input, output,
      expectedNumExprsFound: 3,
      exprToExtractIndex: 0,
      randomizerChoices: new() { true });
  }

  [TestMethod]
  public void Test5() {
    var input = """
    datatype Entity = Person(name: string, age: int) | Animal(name: string, age: int) | Food(name: string)

    method M() {
      var tim := Person("Tim", 10);
      var tam := tim.(name := "Tam", age := 5);
    }
    """;
    var output = """
    datatype Entity = Person(name: string, age: int) | Animal(name: string, age: int) | Food(name: string)

    method M() {
      var tim := Entity.Person("Tim", 10);
      var tam := fn3_mock(tim, "Tam", 5);
    }

    function fn3_mock(fl0_mock: Entity, fl1_mock: string, fl2_mock: int): Entity
      requires fl0_mock.Person? || fl0_mock.Animal?
    {
      fl0_mock.(name := fl1_mock, age := fl2_mock)
    }
    """;
    TestExprExtraction(input, output,
      expectedNumExprsFound: 7,
      exprToExtractIndex: 3,
      randomizerChoices: new() { false, true, true });
  }

  [TestMethod]
  public void Test6() {
    var input = """
    class C {
      var x: int
    }

    method M() {
      var c := new C;
      var x := c.x;
    }
    """;
    var output = """
    class C {
      var x: int
    }

    method M() {
      var c := new C;
      var x := fn1_mock(c);
    }

    function fn1_mock(fl0_mock: C): int
      reads fl0_mock
    {
      fl0_mock.x
    }
    """;
    TestExprExtraction(input, output,
      expectedNumExprsFound: 2,
      exprToExtractIndex: 0,
      randomizerChoices: new() { false, false });
  }

  [TestMethod]
  public void Test7() {
    var input = """
    class C {
      var x: int
    }

    method M() {
      var c := new C;
      var x := c.x;
    }
    """;
    var output = """
    class C {
      var x: int
      function fn0_mock(): int
        reads this
      {
        this.x
      }
    }

    method M() {
      var c := new C;
      var x := c.fn0_mock();
    }
    """;
    TestExprExtraction(input, output,
      expectedNumExprsFound: 2,
      exprToExtractIndex: 0,
      randomizerChoices: new() { false, true });
  }

  [TestMethod]
  public void Test8() {
    var input = """
    method M(b: bool) {
      var i := if b then 1 else 0;
    }
    """;
    // Ok to decompose ITEExpr if there are no unsafe operations.
    var output = """
    method M(b: bool) {
      var i := fn1_mock(b);
    }
    
    function fn1_mock(fl0_mock: bool): int {
      if fl0_mock then 1 else 0
    }
    """;
    TestExprExtraction(input, output,
      expectedNumExprsFound: 4,
      exprToExtractIndex: 0,
      randomizerChoices: new() { false, false, false });
  }

  [TestMethod]
  public void Test9() {
    var input = """
    datatype Access = Guard(danger: bool) | Safe

    method M(b: bool) {
      var a := Guard(b);
      var i := if a.danger then 1 else 0;
    }
    """;
    // Ok to decompose ITEExpr if the unsafe operation only occurs in the guard.
    // Add a precondition to the extracted function.
    var output = """
    datatype Access = Guard(danger: bool) | Safe
    
    method M(b: bool) {
      var a := Access.Guard(b);
      var i := fn1_mock(a);
    }

    function fn1_mock(fl0_mock: Access): int
      requires fl0_mock.Guard?
    {
      if fl0_mock.danger then 1 else 0
    }
    """;
    TestExprExtraction(input, output,
      expectedNumExprsFound: 7,
      exprToExtractIndex: 2,
      randomizerChoices: new() { false, false, false, false, false });
  }

  [TestMethod]
  public void Test10() {
    var input = """
    datatype Access = Guard(danger: int) | Safe

    method M(b: bool) {
      var a := Access.Guard(1);
      var i := if b then a.danger else 0;
    }
    """;
    // Do not decompose ITEExpr if there is at least one unsafe operation in 
    // any of the branches. Pass in the entire ITEExpr by value.
    var output = """
    datatype Access = Guard(danger: int) | Safe
    
    method M(b: bool) {
      var a := Access.Guard(1);
      var i := fn2_mock(if b then a.danger else 0);
    }

    function fn2_mock(fl1_mock: int): int {
      fl1_mock
    }
    """;
    TestExprExtraction(input, output,
      expectedNumExprsFound: 7,
      exprToExtractIndex: 2,
      randomizerChoices: new() { false, false });
  }

  [TestMethod]
  public void Test11() {
    var input = """
    method M() {
      var a := new int[3]((i) => i);
      var i := a[0];
    }
    """;
    var output = """
    method M() {
      var a := new int[3]((i) => i);
      var i := fn2_mock(a, 0);
    }

    function fn2_mock(fl0_mock: array<int>, fl1_mock: int): int
      requires 0 <= fl1_mock && fl1_mock < fl0_mock.Length
      reads fl0_mock
    {
      fl0_mock[fl1_mock]
    }
    """;
    TestExprExtraction(input, output,
      expectedNumExprsFound: 6,
      exprToExtractIndex: 3,
      randomizerChoices: new() { false, true });
  }

  [TestMethod]
  public void Test12() {
    var input = """
    method M() {
      var m := map[0 := 1];
      var i := m[0];
    }
    """;
    var output = """
    method M() {
      var m := map[0 := 1];
      var i := fn2_mock(m, 0);
    }

    function fn2_mock(fl0_mock: map<int, int>, fl1_mock: int): int
      requires fl1_mock in fl0_mock
    {
      fl0_mock[fl1_mock]
    }
    """;
    TestExprExtraction(input, output,
      expectedNumExprsFound: 6,
      exprToExtractIndex: 3,
      randomizerChoices: new() { false, true });
  }

  [TestMethod]
  public void Test13() {
    var input = """
    method M() {
      var s := [1, 2];
      var i := s[0];
    }
    """;
    var output = """
    method M() {
      var s := [1, 2];
      var i := fn2_mock(s, 0);
    }

    function fn2_mock(fl0_mock: seq<int>, fl1_mock: int): int
      requires 0 <= fl1_mock && fl1_mock < |fl0_mock|
    {
      fl0_mock[fl1_mock]
    }
    """;
    TestExprExtraction(input, output,
      expectedNumExprsFound: 6,
      exprToExtractIndex: 3,
      randomizerChoices: new() { false, true });
  }

  [TestMethod]
  public void Test14() {
    var input = """
    method M() {
      var m := multiset{0, 0};
      var i := m[0];
    }
    """;
    var output = """
    method M() {
      var m := multiset{0, 0};
      var i := fn2_mock(m, 0);
    }

    function fn2_mock(fl0_mock: multiset<int>, fl1_mock: int): nat {
      fl0_mock[fl1_mock]
    }
    """;
    TestExprExtraction(input, output,
      expectedNumExprsFound: 6,
      exprToExtractIndex: 3,
      randomizerChoices: new() { false, true });
  }

  [TestMethod]
  public void Test15() {
    var input = """
    method M() {
      var m := map[0 := [1, 2]];
      var i := m[0][1];
    }
    """;
    var output = """
    method M() {
      var m := map[0 := [1, 2]];
      var i := fn2_mock(m, 0);
    }

    function fn2_mock(fl0_mock: map<int, seq<int>>, fl1_mock: int): int
      requires fl1_mock in fl0_mock
      requires 0 <= 1 && 1 < |fl0_mock[fl1_mock]|
    {
      fl0_mock[fl1_mock][1]
    }
    """;
    TestExprExtraction(input, output,
      expectedNumExprsFound: 10,
      exprToExtractIndex: 5,
      randomizerChoices: new() { false, false, true, false });
  }
}
