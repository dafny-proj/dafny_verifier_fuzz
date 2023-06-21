using System.CommandLine;

namespace MutantGenerator;

public class MutantGeneratorMain {
  public static void Main(string[] args) {
    var seedArgument = new Argument<FileInfo>(
      name: "seed",
      description: "The seed file from which mutants are generated.");
    var workdirArgument = new Argument<DirectoryInfo>(
      name: "work-dir",
      description: "The directory to output mutants and logs to.");

    var rootCommand = new RootCommand("Mutant generator for Dafny programs.");
    rootCommand.AddArgument(seedArgument);
    rootCommand.AddArgument(workdirArgument);

    var genSingleCmd = new Command(
      name: "gen-single",
      description: "Generate a single mutant.");
    var mutantSeedOption = new Option<int>(
      name: "--mutant-seed",
      description: "Random seed for determining the mutations applied.");
    var mutantOrderOption = new Option<int>(
      name: "--mutant-order",
      description: "Number of mutations to apply.");
    rootCommand.AddCommand(genSingleCmd);
    genSingleCmd.AddOption(mutantSeedOption);
    genSingleCmd.AddOption(mutantOrderOption);
    genSingleCmd.SetHandler(GenSingle, seedArgument, workdirArgument,
      mutantSeedOption, mutantOrderOption);

    var genMultipleCmd = new Command(
      name: "gen-multi",
      description: "Generate multiple mutants.");
    var numMutantsOption = new Option<int>(
      name: "--num-mutants",
      description: "The number of mutants to generate.",
      getDefaultValue: () => 1);
    var maxOrderOption = new Option<int>(
      name: "--max-order",
      description: "The maximum number of mutations applied to generate a mutant.",
      getDefaultValue: () => 1);
    rootCommand.AddCommand(genMultipleCmd);
    genMultipleCmd.AddOption(numMutantsOption);
    genMultipleCmd.AddOption(maxOrderOption);
    genMultipleCmd.SetHandler(GenMultiple, seedArgument, workdirArgument,
      numMutantsOption, maxOrderOption);

    rootCommand.Invoke(args);
  }

  public static void GenSingle(FileInfo seed, DirectoryInfo workdir,
  int mutantSeed, int mutantOrder) {
    new MutantGenerator(seed.FullName, workdir.FullName)
      .GenerateMutant(mutantSeed, mutantOrder);
  }

  public static void GenMultiple(FileInfo seed, DirectoryInfo workdir,
  int numMutants, int maxOrder) {
    new MutantGenerator(seed.FullName, workdir.FullName)
      .GenerateMutants(numMutants, maxOrder);
  }

}