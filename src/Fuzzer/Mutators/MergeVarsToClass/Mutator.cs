namespace Fuzzer;

// Requires variables to have known types (i.e. not a TypeProxy).
// Requires variable types to be auto-initialisable types.
/// <summary>
/// var x, y;
/// Rewrites to
/// ```
/// class C {
///   var x;
///   var y;
/// }
/// ```
/// </summary>
public class MergeVarsToClassMutation : IMutation {
  public List<LocalVar> Vars;
  public ModuleDecl EnclosingModule;
  public BlockStmt EnclosingScope;

  public MergeVarsToClassMutation(IEnumerable<LocalVar> vars,
  ModuleDecl enclosingModule, BlockStmt enclosingScope) {
    Vars = new(vars);
    EnclosingModule = enclosingModule;
    EnclosingScope = enclosingScope;
  }
}

public class MergeVarsToClassMutator : BasicMutator<MergeVarsToClassMutation> {
  public IGenerator Gen { get; }
  public MergeVarsToClassMutator(Randomizer rand, IGenerator gen) : base(rand) {
    Gen = gen;
  }

  public override List<MergeVarsToClassMutation>
  FindPotentialMutations(Program p) {
    return MergeVarsToClassMutationFinder.FindMutations(p);
  }

  public override MergeVarsToClassMutation
  SelectMutation(List<MergeVarsToClassMutation> ms) {
    Contract.Requires(ms.Count > 0);
    // Select a scope at random.
    var m = Rand.RandElement<MergeVarsToClassMutation>(ms);
    // Select variables at random.
    var vs = Rand.ShuffledSubset<LocalVar>(m.Vars);
    return new MergeVarsToClassMutation(
      vars: vs,
      enclosingScope: m.EnclosingScope,
      enclosingModule: m.EnclosingModule);
  }

  public override void ApplyMutation(MergeVarsToClassMutation m) {
    new MergeVarsToClassMutationRewriter(m, Gen).Rewrite();
  }
}
