using System.Runtime.ExceptionServices;

namespace MutantGenerator;

public class NoMutationsException : Exception {
  public NoMutationsException() : base("No mutations found.") { }
}

public class MutantGenerator {
  private string seedFile;
  private string seedName;
  private Program? seedProgram;

  private string workDir;
  private string outDir;

  private Random globalRand = new Random();
  private Randomizer mutatorRand = new Randomizer();
  private BasicGenerator gen = new BasicGenerator();
  private ComposedMutator mutator;

  public MutantGenerator(string seedFile, string workDir) {
    this.seedFile = seedFile;
    this.seedName = Path.GetFileNameWithoutExtension(seedFile);

    this.workDir = workDir;
    this.outDir = Path.Join(this.workDir, this.seedName);
    Directory.CreateDirectory(this.workDir);
    Directory.CreateDirectory(this.outDir);

    var basicMutators = new List<IBasicMutator>() {
      new ForLoopToWhileLoopMutator(mutatorRand),
      new WhileLoopPeelMutator(mutatorRand, gen),
      new MergeVarsToClassMutator(mutatorRand, gen),
      new MergeVarsToMapMutator(mutatorRand, gen),
      new ExprExtractionMutator(mutatorRand, gen),
    };
    this.mutator = new ComposedMutator(basicMutators, mutatorRand);

    TryParseProgram();
  }

  private void TryParseProgram() {
    try {
      this.seedProgram = DafnyW.ParseProgramFromFile(this.seedFile);
    } catch (Exception e) {
      var logger = new SingleLogger(this.seedName,
        Path.Join(this.outDir, $"{this.seedName}.dfy.log"));
      if (e is DafnyException de) {
        de.ErrorMessages.ForEach(m => logger.LogError(m));
      } else {
        logger.LogError(e.Message);
        if (e.StackTrace != null) { logger.LogError(e.StackTrace); }
      }
      logger.Close();
    }
  }

  public void GenerateMutant(int seed, int order) {
    if (this.seedProgram == null) { return; }
    var mutantFile = Path.Join(this.outDir, $"{this.seedName}_{seed}_{order}.dfy");
    var mutantLogger = new SingleLogger(this.seedName, mutantFile + ".log");
    mutantLogger.LogCheckPoint($"Generating mutant for `{this.seedName}`.");
    mutantLogger.LogCheckPoint($"Seed: {seed}.");
    mutantLogger.LogCheckPoint($"Order: {order}.");
    mutator.ClearHistory();
    mutatorRand.ResetRandomizer(seed);
    try {
      var mutant = Cloner.Clone<Program>(this.seedProgram);
      for (int i = 0; i < order; i++) {
        gen.Reset(suffix: i.ToString());
        // Exit if no more mutations to be applied.
        if (!mutator.TryMutateProgram(mutant)) { break; }
        mutantLogger.LogCheckPoint($"Applied `{mutator.History.Last().GetType()}`.");
      }
      var actualOrder = mutator.History.Count;
      if (actualOrder == 0) { throw new NoMutationsException(); }
      mutantLogger.LogCheckPoint($"Completed applying mutations. {actualOrder} mutations applied.");
      if (File.Exists(mutantFile)) {
        mutantLogger.LogCheckPoint($"Overwriting existing file.");
      }
      using (StreamWriter wr = File.CreateText(mutantFile)) {
        Printer.PrintNode(mutant, wr);
      }
      mutantLogger.LogCheckPoint($"Mutant written to `{mutantFile}`.");
    } catch (Exception e) {
      mutantLogger.LogError(e.Message);
      if (e.StackTrace != null) { mutantLogger.LogError(e.StackTrace); }
    }
    mutantLogger.Close();
  }

  public void GenerateMutants(int numMutants, int maxOrder) {
    if (this.seedProgram == null) { return; }
    for (int i = 0; i < numMutants; i++) {
      var mutantSeed = globalRand.Next();
      var mutantOrder = SelectOrderForMutant(maxOrder);
      GenerateMutant(mutantSeed, mutantOrder);
    }
  }

  private int SelectOrderForMutant(int maxOrder) {
    var min = 1;
    var max = maxOrder;
    if (maxOrder > 10) {
      var p = globalRand.NextDouble();
      if (p < 0.2) {
        max = (int)Math.Floor(0.1 * maxOrder);
      } else if (p < 0.7) {
        min = (int)Math.Ceiling(0.1 * maxOrder);
        max = (int)Math.Floor(0.5 * maxOrder);
      } else if (p < 0.9) {
        min = (int)Math.Ceiling(0.5 * maxOrder);
        max = (int)Math.Floor(0.8 * maxOrder);
      } else {
        min = (int)Math.Ceiling(0.8 * maxOrder);
      }
    }
    return globalRand.Next(minValue: min, maxValue: max + 1);
  }
}