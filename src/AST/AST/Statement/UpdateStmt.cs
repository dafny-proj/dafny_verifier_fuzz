using System.Diagnostics.Contracts;

namespace AST;

public abstract class ConcreteUpdateStatement
: Statement, ConstructableFromDafny<Dafny.ConcreteUpdateStatement, ConcreteUpdateStatement> {
  public List<Expression> Lhss = new List<Expression>();

  protected ConcreteUpdateStatement(IEnumerable<Expression> lhss) {
    Lhss.AddRange(lhss);
  }
  protected ConcreteUpdateStatement(Expression lhs) {
    Lhss.Add(lhs);
  }

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

  public UpdateStmt(IEnumerable<Expression> lhss, IEnumerable<AssignmentRhs> rhss)
  : base(lhss) {
    Rhss.AddRange(rhss);
  }

  public UpdateStmt(Expression lhs, AssignmentRhs rhs) : base(lhs) {
    Rhss.Add(rhs);
  }

  private UpdateStmt(Dafny.UpdateStmt updateStmtDafny) : base(updateStmtDafny) {
    Rhss.AddRange(updateStmtDafny.Rhss.Select(AssignmentRhs.FromDafny));
  }

  public static UpdateStmt FromDafny(Dafny.UpdateStmt dafnyNode) {
    return new UpdateStmt(dafnyNode);
  }

  public override Statement Clone() {
    return new UpdateStmt(
      Lhss.Select(l => l.Clone()),
      Rhss.Select(r => r.Clone())
    );
  }
}