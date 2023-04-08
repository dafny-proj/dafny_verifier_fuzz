using Dafny = Microsoft.Dafny;

namespace DafnyWrappers;
public static class DafnyWrappers
{
  public static readonly string[] VerificationArgs = { "verify", "--cores=2", "--use-basename-for-filename", "--verification-time-limit=300" };
  public static void ParseDafnyProgram(string programFile, out Dafny.Program programDafny)
  {
    // FIXME: For now, assume no errors while parsing
    // FIXME: The pipeline does more work than is required here, it is currently
    //        used to get things going, but we should look into extracting just 
    //        the functionality needed (e.g. removing CLI args processing)
    var cliArgumentsResult = Dafny.DafnyDriver.ProcessCommandLineArguments(
      VerificationArgs.Append(programFile).ToArray(),
      out var dafnyOptions,
      out var dafnyFiles,
      out var otherFiles);
    var reporter = new Dafny.ConsoleErrorReporter(dafnyOptions);
    Dafny.Main.Parse(dafnyFiles, programFile, reporter, out programDafny);
  }

  public static string DafnyProgramToString(Dafny.Program programDafny)
  {
    var writer = new StringWriter();
    var printer = new Dafny.Printer(writer, programDafny.Options, programDafny.Options.PrintMode);
    printer.PrintProgram(programDafny, /*afterResolver=*/false);
    return writer.ToString();
  }

}
