namespace Fuzzer_new;

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
public partial class MergeVarsToMapMutation {
  public List<LocalVar> VarsToMerge;
  public BlockStmt EnclosingScope;
  public IGenerator Generator;

  public MergeVarsToMapMutation(IEnumerable<LocalVar> varsToMerge,
  BlockStmt enclosingScope, IGenerator generator) {
    VarsToMerge = varsToMerge.ToList();
    EnclosingScope = enclosingScope;
    Generator = generator;
  }

  public void Apply() {
    MergeVarsToMapRewriter.ExecuteMutation(this);
  }
}

public partial class MergeVarsToMapRewriter {
  private List<LocalVar> vars;
  private LocalVar map;
  private MapType mapType;
  private BlockStmt enclosingScope;
  private IGenerator gen;
  private List<Task> rewriteTasks = new();

  public static void ExecuteMutation(MergeVarsToMapMutation m) {
    new MergeVarsToMapRewriter(m).Rewrite();
  }

  private MergeVarsToMapRewriter(MergeVarsToMapMutation m) {
    this.gen = m.Generator;
    this.enclosingScope = m.EnclosingScope;
    this.vars = m.VarsToMerge;
    var valueType = GetCommonType(this.vars);
    if (valueType == null) {
      throw new ArgumentException(
        $"Only variables of the same type can be merged into a map.");
    }
    this.mapType = new MapType(Type.String, valueType);
    this.map = GenMapVariable(mapType);
  }

  private Type? GetCommonType(List<LocalVar> vars) {
    if (vars.Count() > 0) {
      var type = vars[0].Type;
      var common = vars.All(v => type == v.Type);
      return common ? type : null;
    }
    return null;
  }

  private void Rewrite() {
    // Insert map declaration in the enclosing scope.
    this.enclosingScope.Prepend(new VarDeclStmt(this.map));
    // Rewrite all defs and uses to the variable in the enclosing scope.
    VisitNode(this.enclosingScope);
    rewriteTasks.ForEach(t => t.Execute());
  }

  private bool ContainsVar(LocalVar v) => vars.Contains(v);
  private LocalVar? TryGetAffectedVar(Expression e) {
    if (e is IdentifierExpr i && i.Var is LocalVar lv && ContainsVar(lv)) {
      return lv;
    }
    return null;
  }
}
