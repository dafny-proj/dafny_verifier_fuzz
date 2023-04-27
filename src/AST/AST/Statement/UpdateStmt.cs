namespace AST;

public abstract class ConcreteUpdateStatement
: Statement, ConstructableFromDafny<Dafny.ConcreteUpdateStatement, ConcreteUpdateStatement> {
  public List<Expression> Lhss = new List<Expression>();

  protected ConcreteUpdateStatement(Dafny.ConcreteUpdateStatement cuStmtDafny) {
    Lhss.AddRange(cuStmtDafny.Lhss.Select(Expression.FromDafny));
  }

  public static ConcreteUpdateStatement FromDafny(Dafny.ConcreteUpdateStatement cuStmtDafny) {
    return cuStmtDafny switch {
      Dafny.UpdateStmt us => UpdateStmt.FromDafny(us),
      _ => throw new NotImplementedException(),
    };
  }
}

public class UpdateStmt
: ConcreteUpdateStatement, ConstructableFromDafny<Dafny.UpdateStmt, UpdateStmt> {
  public override IEnumerable<Node> Children => Lhss.Concat<Node>(Rhss);

  public List<AssignmentRhs> Rhss = new List<AssignmentRhs>();

  private UpdateStmt(Dafny.UpdateStmt updateStmtDafny) : base(updateStmtDafny) {
    Rhss.AddRange(updateStmtDafny.Rhss.Select(AssignmentRhs.FromDafny));
    // TODO: should this somehow use updateStmtDafny.ResolvedStatements?
  }

  public static UpdateStmt FromDafny(Dafny.UpdateStmt dafnyNode) {
    return new UpdateStmt(dafnyNode);
  }
}