namespace AST;

// TODO: After resolution, an UpdateStmt is created, do we use that instead of Rhss?
public class ReturnStmt
: Statement, ConstructableFromDafny<Dafny.ReturnStmt, ReturnStmt> {
  public override IEnumerable<Node> Children => Rhss;

  public List<AssignmentRhs> Rhss = new List<AssignmentRhs>();

  private ReturnStmt(Dafny.ReturnStmt retStmtDafny) {
    Rhss.AddRange(retStmtDafny.Rhss.Select(AssignmentRhs.FromDafny));
  }

  public static ReturnStmt FromDafny(Dafny.ReturnStmt dafnyNode) {
    return new ReturnStmt(dafnyNode);
  }
}
