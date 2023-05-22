namespace AST_new;

public partial class BreakStmt : Statement { }
public partial class ContinueStmt : BreakStmt { }

public partial class BreakStmt : Statement {
  public int Count { get; }
  public string? TargetLabel { get; }

  public bool HasTargetLabel() => TargetLabel != null;

  private BreakStmt(int _count = 1, string? _targetLabel = null) {
    Count = _count;
    TargetLabel = _targetLabel;
  }

  public BreakStmt(int count) : this(_count: count) { }
  public BreakStmt(string targetLabel) : this(_targetLabel: targetLabel) { }

  public override IEnumerable<Node> Children => Enumerable.Empty<Node>();
}

public partial class ContinueStmt : BreakStmt {
  public ContinueStmt(int count) : base(count) { }
  public ContinueStmt(string targetLabel) : base(targetLabel) { }
}
