namespace AST_new;

public partial class VarDeclStmt : Statement {
  public readonly List<LocalVar> Vars = new();
  public UpdateStmt? Initialiser { get; }

  public VarDeclStmt(IEnumerable<LocalVar> vars,
  UpdateStmt? initialiser = null) {
    Vars.AddRange(vars);
    Initialiser = initialiser;
  }
}
