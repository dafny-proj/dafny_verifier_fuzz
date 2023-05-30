using Dafny = Microsoft.Dafny;
using System.Diagnostics;

namespace DafnyWrappers;

public class DafnyException : Exception {
  public int ErrorCount = 0;
  public List<string> ErrorMessages = new();
  public DafnyException(Dafny.BatchErrorReporter reporter) {
    ErrorCount = reporter.Count(Dafny.ErrorLevel.Error);
    foreach (var err in reporter.AllMessages[Dafny.ErrorLevel.Error]) {
      ErrorMessages.Add(
        Dafny.ErrorReporter.ErrorToString(err.Level, err.Token, err.Message));
    }
  }
  public DafnyException(int errorCount) {
    ErrorCount = errorCount;
  }
}

public class DafnyParseException : DafnyException {
  public DafnyParseException(Dafny.BatchErrorReporter reporter)
  : base(reporter) { }
  public DafnyParseException(int errorCount) : base(errorCount) { }
}

public class DafnyResolveException : DafnyException {
  public DafnyResolveException(Dafny.BatchErrorReporter reporter)
  : base(reporter) { }
  public DafnyResolveException(int errorCount) : base(errorCount) { }
}

public static partial class DafnyWrappers {
  public static AST.Program ParseProgramFromFile(string filepath) {
    return ParseProgramFromString(File.ReadAllText(filepath), filepath);
  }

  public static AST.Program ParseProgramFromString(string program, string filepath="program.dfy") {
    var programDafny = ParseDafnyProgramFromString(program, filepath);
    ResolveDafnyProgram(programDafny);
    return AST.Translation.ASTTranslator.TranslateDafnyProgram(programDafny);
  }

  public static Dafny.Program ParseDafnyProgramFromFile(string filepath) {
    var program = File.ReadAllText(filepath);
    return ParseDafnyProgramFromString(program, filepath);
  }

  public static Dafny.Program
  ParseDafnyProgramFromString(string program, string filePath = "program.dfy") {
    var filename = Path.GetFileName(filePath);
    var options = Dafny.DafnyOptions.Create();
    var module = new Dafny.LiteralModuleDecl(
      new Dafny.DefaultModuleDefinition(), /*parent=*/null);
    var builtIns = new Dafny.BuiltIns(options);
    var reporter = new Dafny.BatchErrorReporter(options);
    var numParseErrors = Dafny.Parser.Parse(
      program, filePath, filename, module, builtIns, reporter);
    if (numParseErrors > 0) {
      throw new DafnyParseException(reporter);
    }
    return new Dafny.Program(filename, module, builtIns, reporter);
  }

  public static void ResolveDafnyProgram(Dafny.Program programDafny) {
    var resolver = new Dafny.Resolver(programDafny);
    resolver.ResolveProgram(programDafny);
    var reporter = programDafny.Reporter;
    var numResolveErrors = reporter.ErrorCountUntilResolver;
    if (numResolveErrors > 0) {
      var exception = reporter is Dafny.BatchErrorReporter r ?
        new DafnyResolveException(r) :
        new DafnyResolveException(numResolveErrors);
      throw exception;
    }
  }
}

// Internal tools for development.
public static partial class DafnyWrappers {
  public static string DafnyProgramToString(
  Dafny.Program programDafny, bool postResolution = false) {
    var writer = new StringWriter();
    var printer = new Dafny.Printer(writer,
      programDafny.Options, programDafny.Options.PrintMode);
    printer.PrintProgram(programDafny, postResolution);
    return writer.ToString();
  }

  public const string DefaultDafnyPath = "dafny_proj/dafny/Scripts/dafny";
  public static readonly string[] DefaultVerificationArgs = {
    "verify",
    "--cores=2",
    "--use-basename-for-filename",
    "--verification-time-limit=300" };

  public static (int, string, string)
  RunDafnyVerify(string filepath, string dafnyPath = DefaultDafnyPath) {
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

  public static void PrintDafnyAST(
  Dafny.Node node, string prefix = "0", bool preResolve = false) {
    Console.WriteLine($"{prefix}: {node.GetType()}");
    int childCount = 1;
    var children = preResolve ? node.PreResolveChildren : node.Children;
    foreach (var child in children) {
      PrintDafnyAST(child, $"{prefix}.{childCount++}");
    }
  }

}
