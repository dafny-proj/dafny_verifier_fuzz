namespace Fuzzer;

// Requires variables to have the same type.
// Requires variables to not be assigned values from side-effecting operations,
// e.g. method calls, array/object creation.
/// <summary>
/// var x, y;
/// Rewrites to
/// ```
/// map[x := ..., y := ...]
/// ```
/// </summary>
public class MergeVarsToMapMutation : IMutation {
  public List<LocalVar> Vars;
  public BlockStmt EnclosingScope;

  public MergeVarsToMapMutation(
  IEnumerable<LocalVar> vars, BlockStmt enclosingScope) {
    Vars = new(vars);
    EnclosingScope = enclosingScope;
  }
}

public class MergeVarsToMapMutator : BasicMutator<MergeVarsToMapMutation> {
  public IGenerator Gen { get; }
  public MergeVarsToMapMutator(Randomizer rand, IGenerator gen) : base(rand) {
    Gen = gen;
  }

  public override List<MergeVarsToMapMutation>
  FindPotentialMutations(Program p) {
    var potentialMutations = new List<MergeVarsToMapMutation>();
    var scopes = LocalVarTrackingScopeBuilder.FindScopes(p);
    var blackList = VarWithSideEffectingAssignmentFinder.Find(p);
    foreach (var s in scopes) {
      // Remove variables with side-effecting assignments or unknown type.
      var validVars = s.Vars.Where(v =>
        !blackList.Contains(v) && v.Type is not TypeProxy);
      // Group variables by type.
      var typedVars = new Dictionary<Type, List<LocalVar>>();
      foreach (var v in validVars) {
        var vt = v.Type;
        if (!typedVars.ContainsKey(vt)) { typedVars.Add(vt, new()); }
        typedVars[vt].Add(v);
      }
      // Variables of the same scope and type can be merged to a map.
      foreach (var (_, vs) in typedVars) {
        potentialMutations.Add(new MergeVarsToMapMutation(
          vars: vs,
          enclosingScope: s.Node));
      }
    }
    return potentialMutations;
  }

  public override MergeVarsToMapMutation
  SelectMutation(List<MergeVarsToMapMutation> ms) {
    Contract.Requires(ms.Count > 0);
    // Select a scope and type at random.
    var m = Rand.RandElement<MergeVarsToMapMutation>(ms);
    // Select variables at random.
    var vs = m.Vars.Where(_ => Rand.RandBool());
    return new MergeVarsToMapMutation(
      vars: vs,
      enclosingScope: m.EnclosingScope);
  }

  public override void ApplyMutation(MergeVarsToMapMutation m) {
    new MergeVarsToMapMutationRewriter(m, Gen).Rewrite();
  }
}
