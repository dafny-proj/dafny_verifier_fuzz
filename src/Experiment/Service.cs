namespace Fuzzer;

public class NoMutationsException : Exception {
  public NoMutationsException() : base("No mutations found.") { }
}

public static class FuzzerService {
  public static string ParseBaseName(string filePath) {
    return Path.GetFileNameWithoutExtension(filePath);
  }

  public static string PrintProgramToString(Program program) {
    return Printer.PrintNodeToString(program);
  }

  public static void PrintProgramToFile(Program program, string filePath) {
    if (File.Exists(filePath)) {
      throw new Exception($"File `{filePath}` already exists.");
    }
    using (StreamWriter wr = File.CreateText(filePath)) {
      Printer.PrintNode(program, wr);
    }
  }

  public static void GenerateMutantsAsFile(string seedFilePath,
  string mutantOutDir, int numOfMutants = 1, int maxOrder = 1) {
    var seedName = ParseBaseName(seedFilePath);
    var seedProgram = DafnyW.ParseProgramFromFile(seedFilePath);
    var mutants = GenerateMutants(seedProgram, numOfMutants, maxOrder);
    if (!Directory.Exists(mutantOutDir)) {
      Directory.CreateDirectory(mutantOutDir);
    }
    for (int i = 0; i < mutants.Count(); i++) {
      var mutantFilePath = Path.Combine(mutantOutDir, $"{seedName}_{i}.dfy");
      PrintProgramToFile(mutants.ElementAt(i), mutantFilePath);
    }
  }

  public static IEnumerable<string>
  GenerateMutantsAsString(string seed, int numOfMutants = 1, int maxOrder = 1) {
    var seedProgram = DafnyW.ParseProgramFromString(seed);
    var mutants = GenerateMutants(seedProgram, numOfMutants, maxOrder);
    foreach (var mutant in mutants) {
      yield return PrintProgramToString(mutant);
    }
  }

  public static IEnumerable<Program>
  GenerateMutants(Program seed, int numOfMutants = 1, int maxOrder = 1) {
    for (int i = 0; i < numOfMutants; i++) {
      yield return GenerateMutant(seed, maxOrder);
    }
  }

  public static Program GenerateMutant(Program seed, int maxOrder = 1) {
    var rand = new Randomizer();
    var gen = new BasicGenerator();

    var basicMutators = new List<IBasicMutator>() {
      new ForLoopToWhileLoopMutator(rand),
      new WhileLoopUnpeelMutator(rand, gen),
      new MergeVarsToClassMutator(rand, gen),
      new MergeVarsToMapMutator(rand, gen),
    };
    var mutator = new ComposedMutator(basicMutators, rand);
    var mutant = Cloner.Clone<Program>(seed);

    int order = rand.RandInt(minValue: 1, maxValue: maxOrder + 1);
    int numOfMutationsApplied = 0;
    for (int i = 0; i < order; i++) {
      gen.Reset(suffix: i.ToString());
      // Exit if no more mutations to be applied.
      if (!mutator.TryMutateProgram(mutant)) { break; }
      numOfMutationsApplied++;
    }
    if (numOfMutationsApplied == 0) {
      throw new NoMutationsException();
    }
    return mutant;
  }
}