namespace AST_new;

public abstract partial class UpdateStmt : Statement { }
// e.g. a, b, c := E1, E2, E3;
public partial class AssignStmt : UpdateStmt { }
// e.g. a, b, c := Call();
public partial class CallStmt : UpdateStmt { }

public abstract partial class UpdateStmt : Statement {
  public abstract IReadOnlyList<Expression> Lhss { get; }
  public abstract IReadOnlyList<AssignmentRhs> Rhss { get; }
}

public partial class AssignStmt : UpdateStmt {
  private List<AssignmentPair> _assignments = new();

  public override IReadOnlyList<Expression> Lhss
    => _assignments.Select(a => a.Key).ToList().AsReadOnly();
  public override IReadOnlyList<AssignmentRhs> Rhss
    => _assignments.Select(a => a.Value).ToList().AsReadOnly();

  public AssignStmt(IEnumerable<AssignmentPair>? assignments) {
    if (assignments != null) {
      _assignments.AddRange(assignments);
    }
  }
}

public partial class CallStmt : UpdateStmt {
  private List<Expression> _lhss = new();
  private MethodCallRhs _rhs { get; }

  public override IReadOnlyList<Expression> Lhss
    => _lhss.AsReadOnly();
  public override IReadOnlyList<AssignmentRhs> Rhss
    => (new[] { _rhs }).AsReadOnly();

  public CallStmt(MethodCallRhs call, IEnumerable<Expression>? lhss = null) {
    _rhs = call;
    if (lhss != null) {
      _lhss.AddRange(lhss);
    }
  }
}
