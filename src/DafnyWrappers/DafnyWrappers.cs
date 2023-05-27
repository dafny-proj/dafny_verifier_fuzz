using Dafny = Microsoft.Dafny;
using System.Diagnostics;

namespace DafnyWrappers;
public static partial class DafnyWrappers {
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

  public static void
  ResolveDafnyProgram(Dafny.Program programDafny) {
    var resolver = new Dafny.Resolver(programDafny);
    resolver.ResolveProgram(programDafny);
  }

  public static string DafnyProgramToString(Dafny.Program programDafny) {
    var writer = new StringWriter();
    var printer = new Dafny.Printer(writer, programDafny.Options, programDafny.Options.PrintMode);
    printer.PrintProgram(programDafny, /*afterResolver=*/false);
    return writer.ToString();
  }

  public const string DefaultDafnyPath = "dafny_proj/dafny/Scripts/dafny";
  public static readonly string[] DefaultVerificationArgs = {
    "verify",
    "--cores=2",
    "--use-basename-for-filename",
    "--verification-time-limit=300" };

  public static (int, string, string) RunDafnyVerify(string filepath, string dafnyPath = DefaultDafnyPath) {
    // Verification API is not easily callable, use Dafny CLI instead
    var process = new Process();
    process.StartInfo.FileName = dafnyPath;
    foreach (var vArg in DefaultVerificationArgs) {
      process.StartInfo.ArgumentList.Add(vArg);
    }
    process.StartInfo.ArgumentList.Add(filepath);
    process.StartInfo.UseShellExecute = false;
    process.StartInfo.RedirectStandardInput = true;
    process.StartInfo.RedirectStandardOutput = true;
    process.StartInfo.RedirectStandardError = true;
    process.Start();
    string output = process.StandardOutput.ReadToEnd();
    string error = process.StandardError.ReadToEnd();
    process.WaitForExit();
    return (process.ExitCode, output, error);
  }

  public static void PrintDafnyASTByDepth(Dafny.Program programDafny) {
    List<(string, Dafny.Node)> nodes = new List<(string, Dafny.Node)>();
    nodes.Add(("0", programDafny));
    int depth = 0;
    while (nodes.Count() != 0) {
      Console.WriteLine(depth);
      var nextGen = new List<(string, Dafny.Node)>();
      foreach (var (index, node) in nodes) {
        Console.WriteLine($"{index}: {node.GetType()}");
        int count = 1;
        foreach (var child in node.Children) {
          nextGen.Add(($"{index}.{count}", child));
          count++;
        }
      }
      nodes = nextGen;
      depth++;
    }
  }

  public static void PrintDafnyAST(Dafny.Node node, string prefix="0", bool preResolve = false) {
    Console.WriteLine($"{prefix}: {node.GetType()}");
    int childCount = 1;
    var children = preResolve ? node.PreResolveChildren : node.Children;
    foreach (var child in children) {
      PrintDafnyAST(child, $"{prefix}.{childCount++}");
    }
  }

  public static AST.Program ParseProgramFromString(string program) {
    var programDafny = ParseDafnyProgramFromString(program);
    ResolveDafnyProgram(programDafny);
    return AST.Translation.DafnyASTTranslator.TranslateDafnyProgram(programDafny);
  }
  
}
