namespace AST;

public class VarDeclStmt
: Statement, ConstructableFromDafny<Dafny.VarDeclStmt, VarDeclStmt> {
    public override IEnumerable<Node> Children {
      get {
        var children = Locals;
        if (Update != null) {
          children.Append<Node>(Update);
        }
        return children;
      }
    }
  public List<LocalVariable> Locals = new List<LocalVariable>();
  public ConcreteUpdateStatement? Update;

  private VarDeclStmt(Dafny.VarDeclStmt vdStmt) {
    Locals.AddRange(vdStmt.Locals.Select(LocalVariable.FromDafny));
    Update = vdStmt.Update == null ? null : ConcreteUpdateStatement.FromDafny(vdStmt.Update);
  }

  public static VarDeclStmt FromDafny(Dafny.VarDeclStmt dafnyNode) {
    return new VarDeclStmt(dafnyNode);
  }
}