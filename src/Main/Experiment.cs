using DafnyW = DafnyWrappers.DafnyWrappers;

public class Experiment {
  public static int Main(string[] args) {
    var sourceStr = """
    method Double(x: int) returns (r: int)
      requires 0 <= x
      ensures r >= 2 * x
    {
      r := x + x;
    }
    """;

    var programDafny = DafnyW.ParseDafnyProgramFromString(sourceStr);
    DafnyW.PrintDafnyAST(programDafny);
    return 0;
  }
}