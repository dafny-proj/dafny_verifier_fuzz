namespace Microsoft.Dafny {

public class FuzzMain
{
  public static int Main(string[] args)
  {
    args = new string[]{"verify", "examples/sum.dfy"};
    var cliArgumentsResult = DafnyDriver.ProcessCommandLineArguments(args, out var dafnyOptions, out var dafnyFiles, out var otherFiles);
    if (cliArgumentsResult != DafnyDriver.CommandLineArgumentsResult.OK) {
      return -1;
    }
    ErrorReporter reporter = new ConsoleErrorReporter(dafnyOptions);
    Program dafnyProgram;
    Dafny.Main.Parse(dafnyFiles, "the_program", reporter, out dafnyProgram);
    Dafny.Main.MaybePrintProgram(dafnyProgram, "-", false);    
    return 0;
  }
}

}

