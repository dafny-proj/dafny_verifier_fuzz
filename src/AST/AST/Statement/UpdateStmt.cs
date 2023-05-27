namespace AST;

public abstract partial class UpdateStmt : Statement { }
// e.g. a, b, c := E1, E2, E3;
public partial class AssignStmt : UpdateStmt { }
// e.g. a, b, c := Call();
public partial class CallStmt : UpdateStmt { }

public abstract partial class UpdateStmt : Statement {
  public abstract IReadOnlyList<Expression> Lhss { get; }
  public abstract IReadOnlyList<AssignmentRhs> Rhss { get; }

  public bool HasLhs() => Lhss.Count > 0;

  public override IEnumerable<Node> Children => Lhss.Concat<Node>(Rhss);
}

public partial class AssignStmt : UpdateStmt {
  public readonly List<AssignmentPair> Assignments = new();

  public override IReadOnlyList<Expression> Lhss
    => Assignments.Select(a => a.Key).ToList().AsReadOnly();
  public override IReadOnlyList<AssignmentRhs> Rhss
    => Assignments.Select(a => a.Value).ToList().AsReadOnly();

  public AssignStmt() { }
  public AssignStmt(IEnumerable<AssignmentPair> assignments) {
    AddAssignments(assignments);
  }
  public AssignStmt(AssignmentPair assignment) {
    AddAssignment(assignment);
  }

  public void AddAssignment(AssignmentPair assignment)
    => Assignments.Add(assignment);
  public void AddAssignments(IEnumerable<AssignmentPair> assignments)
    => Assignments.AddRange(assignments);

  public override IEnumerable<Node> Children => Assignments;
}

public partial class CallStmt : UpdateStmt {
  public List<Expression> CallLhss = new();
  public MethodCallRhs Call { get; set; }

  public override IReadOnlyList<Expression> Lhss
    => CallLhss.AsReadOnly();
  public override IReadOnlyList<AssignmentRhs> Rhss
    => (new[] { Call }).AsReadOnly();

  public CallStmt(MethodCallRhs call, IEnumerable<Expression>? lhss = null) {
    Call = call;
    if (lhss != null) {
      CallLhss.AddRange(lhss);
    }
  }
}
