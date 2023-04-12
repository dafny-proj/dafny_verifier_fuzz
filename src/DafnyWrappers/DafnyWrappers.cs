using Dafny = Microsoft.Dafny;

namespace DafnyWrappers;
public static class DafnyWrappers {
  public static readonly string[] VerificationArgs = { "verify", "--cores=2", "--use-basename-for-filename", "--verification-time-limit=300" };

  public static Dafny.DafnyOptions DefaultDafnyParseOptions() {
    return Dafny.DafnyOptions.Create();
  }

  // FIXME: For now, assume no errors while parsing and that we only parse a single file
  public static Dafny.Program
  ParseDafnyProgramFromFile(string sourceFile) {
    var sourceStr = File.ReadAllText(sourceFile);
    return ParseDafnyProgramFromString(sourceStr, sourceFile);
  }

  // FIXME: For now, assume no errors while parsing and that we only parse a single file
  public static Dafny.Program
  ParseDafnyProgramFromString(string sourceStr, string sourceFile = "") {
    var options = DefaultDafnyParseOptions();
    var defModule = new Dafny.LiteralModuleDecl(
      new Dafny.DefaultModuleDefinition(), /*parent=*/null);
    var builtIns = new Dafny.BuiltIns(options);
    var reporter = new Dafny.ConsoleErrorReporter(options);
    var success = Dafny.Parser.Parse(sourceStr, sourceFile, sourceFile, defModule, builtIns, reporter);
    return new Dafny.Program("program", defModule, builtIns, reporter);
  }

  public static string DafnyProgramToString(Dafny.Program programDafny) {
    var writer = new StringWriter();
    var printer = new Dafny.Printer(writer, programDafny.Options, programDafny.Options.PrintMode);
    printer.PrintProgram(programDafny, /*afterResolver=*/false);
    return writer.ToString();
  }

}
