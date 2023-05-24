namespace AST_new;

public partial class VarDeclStmt : Statement {
  public readonly List<LocalVar> Vars = new();
  public UpdateStmt? Initialiser { get; set; }

  public bool HasInitialiser() => Initialiser != null;

  public VarDeclStmt(IEnumerable<LocalVar> vars,
  UpdateStmt? initialiser = null) {
    Vars.AddRange(vars);
    Initialiser = initialiser;
  }
  public VarDeclStmt(LocalVar var, UpdateStmt? initialiser = null)
  : this(new[] { var }, initialiser) { }

  public override IEnumerable<Node> Children {
    get {
      foreach (var v in Vars) { yield return v; }
      if (HasInitialiser()) {
        foreach (var r in Initialiser!.Rhss) { yield return r; }
      }
    }
  }
}
