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

    seedOption.IsRequired = true;
    outputDirOption.IsRequired = true;
    numMutantsOption.IsRequired = true;

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
    Directory.CreateDirectory(outputDir.FullName);
    var errorLogger = new SingleLogger(seed.Name,
      Path.Join(outputDir.FullName, "error.log"));
    var exitCode = FuzzerExitCode.Success;
    try {
      new Fuzzer().GenerateMutants(
        seed.FullName, outputDir.FullName, numMutants, maxOrder);
    } catch (Exception e) {
      if (e is DafnyException de) {
        de.ErrorMessages.ForEach(m => errorLogger.LogError(m));
      } else {
        errorLogger.LogError(e.Message);
        if (e.StackTrace != null) { errorLogger.LogError(e.StackTrace); }
      }
      exitCode = e switch {
        DafnyException => FuzzerExitCode.DafnyError,
        AST.Translation.UnsupportedTranslationException => FuzzerExitCode.TranslationError,
        ASTException => FuzzerExitCode.InternalError,
        NoMutationsException => FuzzerExitCode.NoMutationError,
        _ => FuzzerExitCode.OtherError,
      };
    }
    errorLogger.Close();
    Environment.Exit((int)exitCode);
  }

}