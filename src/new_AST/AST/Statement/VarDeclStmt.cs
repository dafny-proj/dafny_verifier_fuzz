namespace AST_new;

public partial class VarDeclStmt : Statement {
  public readonly List<LocalVar> Vars = new();
  public UpdateStmt? Initialiser { get; }

  public bool HasInitialiser() => Initialiser != null;

  public VarDeclStmt(IEnumerable<LocalVar> vars,
  UpdateStmt? initialiser = null) {
    Vars.AddRange(vars);
    Initialiser = initialiser;
  }

  public override IEnumerable<Node> Children {
    get {
      foreach (var v in Vars) { yield return v; }
      if (HasInitialiser()) {
        foreach (var r in Initialiser!.Rhss) { yield return r; }
      }
    }
  }
}
