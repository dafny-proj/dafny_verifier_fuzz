namespace Fuzzer_new;

public interface IMutation { }
public interface IMutator {
  public bool TryMutateProgram(Program p);
}

public class ComposedMutator : IMutator {
  public List<IBasicMutator> Mutators;
  public Randomizer Rand;
  public ComposedMutator(IEnumerable<IBasicMutator> mutators, Randomizer rand) {
    Mutators = new(mutators);
    Rand = rand;
  }

  public bool TryMutateProgram(Program p) {
    Mutators = Rand.Shuffle<IBasicMutator>(Mutators).ToList();
    foreach (var m in Mutators) {
      if (m.TryMutateProgram(p)) { return true; }
    }
    return false;
  }
}

public interface IBasicMutator : IMutator { }

public abstract class BasicMutator<MutationT> : IBasicMutator
where MutationT : IMutation {
  public Randomizer Rand;
  public BasicMutator(Randomizer rand) {
    Rand = rand;
  }

  public bool TryMutateProgram(Program p) {
    var potentialMutations = FindPotentialMutations(p);
    if (potentialMutations.Count == 0) { return false; }
    var mutation = SelectMutation(potentialMutations);
    ApplyMutation(mutation);
    return true;
  }

  public abstract List<MutationT> FindPotentialMutations(Program p);
  public abstract MutationT SelectMutation(List<MutationT> ms);
  public abstract void ApplyMutation(MutationT m);
}
