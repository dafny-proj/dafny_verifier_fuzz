using System.CommandLine;

namespace Fuzzer;

public enum FuzzerExitCode : int {
  Success = 0,
  OtherError = 10,
  DafnyError = 20,
  TranslationError = 30,
  InternalError = 40,
  NoMutationError = 50,
}

public class FuzzerMain {
  public static void Main(string[] args) {
    var seedOption = new Option<FileInfo>(
      name: "--seed",
      description: "The seed file from which mutants are generated.");
    var outputDirOption = new Option<DirectoryInfo>(
      name: "--output-dir",
      description: "The output directory for containing the mutants.");
    var numMutantsOption = new Option<int>(
      name: "--num-mutants",
      description: "The number of mutants to generate.");
    var maxOrderOption = new Option<int>(
      name: "--max-order",
      description: "The maximum number of mutations applied to generate a mutant.",
      getDefaultValue: () => 1);

    var rootCommand = new RootCommand("Mutant generator for Dafny programs.");
    rootCommand.Name = "gen-mutant";
    rootCommand.AddOption(seedOption);
    rootCommand.AddOption(outputDirOption);
    rootCommand.AddOption(numMutantsOption);
    rootCommand.AddOption(maxOrderOption);
    rootCommand.SetHandler(MainHelper, 
      seedOption, outputDirOption, numMutantsOption, maxOrderOption);
    rootCommand.Invoke(args);
  }

  public static void MainHelper(
  FileInfo seed, DirectoryInfo outputDir, int numMutants, int maxOrder) {
    try {
      FuzzerService.GenerateMutantsAsFile(
        seed.FullName, outputDir.FullName, numMutants, maxOrder);
    } catch (Exception e) {
      if (e is DafnyException de) {
        var stage = de switch {
          DafnyParseException => "parse",
          DafnyResolveException => "resolver",
          _ => "",
        };
        Console.WriteLine($"{de.ErrorCount} {stage} errors.");
        de.ErrorMessages.ForEach(m => Console.WriteLine(m));
      } else {
        Console.WriteLine(e.Message);
      }
      var errorCode = e switch {
        DafnyException => FuzzerExitCode.DafnyError,
        AST.Translation.UnsupportedTranslationException => FuzzerExitCode.TranslationError,
        ASTException => FuzzerExitCode.InternalError,
        NoMutationsException => FuzzerExitCode.NoMutationError,
        _ => FuzzerExitCode.OtherError,
      };
      Environment.Exit((int)errorCode);
    }
    Environment.Exit((int)FuzzerExitCode.Success);
  }

}