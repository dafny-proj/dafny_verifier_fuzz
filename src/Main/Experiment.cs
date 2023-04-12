using DafnyW = DafnyWrappers.DafnyWrappers;

public class Experiment {
  public static int Main(string[] args) {
    var programDafny = DafnyW.ParseDafnyProgramFromFile("../../examples/sum.dfy");
    DafnyW.PrintDafnyAST(programDafny);
    return 0;
  }
}