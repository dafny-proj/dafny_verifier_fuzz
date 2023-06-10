using System.Runtime.ExceptionServices;

namespace Fuzzer;

public class NoMutationsException : Exception {
  public NoMutationsException() : base("No mutations found.") { }
}

public class MutantInfo {
  public string SeedFile;
  public Program SeedProgram;
  public int RandSeed;
  public int Order;
  public string MutantFile;
  public ILogger Logger;

  public string SeedName => Path.GetFileName(SeedFile);

  public MutantInfo(string seedFile, Program seedProgram, string mutantFile,
  int randSeed, int order, ILogger logger) {
    SeedFile = seedFile;
    SeedProgram = seedProgram;
    MutantFile = mutantFile;
    RandSeed = randSeed;
    Order = order;
    Logger = logger;
  }

}

public class Fuzzer {
  private Random globalRand = new Random(0);
  private Randomizer mutatorRand = new Randomizer();
  private BasicGenerator gen = new BasicGenerator();
  private ComposedMutator mutator;

  public Fuzzer() {
    var basicMutators = new List<IBasicMutator>() {
      new ForLoopToWhileLoopMutator(mutatorRand),
      new WhileLoopPeelMutator(mutatorRand, gen),
      new MergeVarsToClassMutator(mutatorRand, gen),
      new MergeVarsToMapMutator(mutatorRand, gen),
    };
    mutator = new ComposedMutator(basicMutators, mutatorRand);
  }

  public void GenerateMutant(MutantInfo i) {
    i.Logger.LogCheckPoint($"Generating mutant for `{i.SeedName}`.");
    i.Logger.LogCheckPoint($"Seed: {i.RandSeed}");
    i.Logger.LogCheckPoint($"Order: {i.Order}");
    mutator.ClearHistory();
    mutatorRand.ResetRandomizer(i.RandSeed);
    var mutant = Cloner.Clone<Program>(i.SeedProgram);
    for (int j = 0; j < i.Order; j++) {
      gen.Reset(suffix: j.ToString());
      // Exit if no more mutations to be applied.
      if (!mutator.TryMutateProgram(mutant)) { break; }
      i.Logger.LogCheckPoint($"Applied `{mutator.History.Last().GetType()}`.");
    }
    var numMutationsApplied = mutator.History.Count;
    if (numMutationsApplied == 0) {
      throw new NoMutationsException();
    }
    if (File.Exists(i.MutantFile)) {
      i.Logger.LogCheckPoint($"Overwriting existing file `{i.MutantFile}`.");
    }
    using (StreamWriter wr = File.CreateText(i.MutantFile)) {
      Printer.PrintNode(mutant, wr);
    }
    i.Logger.LogCheckPoint($"Completed generating mutant. {numMutationsApplied} mutations applied.");
    i.Logger.Close();
  }

  public void GenerateMutants(string seedFile, string outdir,
  int numMutants = 1, int maxOrder = 1) {
    var seedName = Path.GetFileNameWithoutExtension(seedFile);
    var seedProgram = DafnyW.ParseProgramFromFile(seedFile);
    var mutantsDir = Path.Join(outdir, seedName);
    Directory.CreateDirectory(mutantsDir);
    for (int i = 0; i < numMutants; i++) {
      var mutantSeed = globalRand.Next();
      var mutantOrder = globalRand.Next(minValue: 1, maxValue: maxOrder + 1);
      var mutantFile
        = Path.Join(mutantsDir, $"{seedName}_{mutantSeed}_{mutantOrder}.dfy");
      var mutantLog = mutantFile + ".log";
      var mutantInfo = new MutantInfo(seedFile, seedProgram, mutantFile,
        mutantSeed, mutantOrder, new SingleLogger(seedName, mutantLog));
      ExceptionDispatchInfo? edi = null;
      try {
        GenerateMutant(mutantInfo);
      } catch (Exception e) {
        mutantInfo.Logger.LogError(e.Message);
        if (e.StackTrace != null) { mutantInfo.Logger.LogError(e.StackTrace); }
        edi = ExceptionDispatchInfo.Capture(e);
      }
      mutantInfo.Logger.Close();
      edi?.Throw();
    }
  }

}