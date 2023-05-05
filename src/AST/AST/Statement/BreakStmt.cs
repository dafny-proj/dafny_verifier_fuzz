namespace AST;

public class BreakStmt
: Statement, ConstructableFromDafny<Dafny.BreakStmt, BreakStmt> {
  public int Count { get; set; }
  public string? TargetLabel { get; set; }

  private BreakStmt(int count = 0, string? label = null) {
    Count = count;
    TargetLabel = label;
  }

  public BreakStmt(int _count) : this(count: _count) { }

  public BreakStmt(string _label) : this(label: _label) { }

  private BreakStmt(Dafny.BreakStmt bsd) {
    Count = bsd.BreakAndContinueCount;
    if (bsd.TargetLabel != null) {
      TargetLabel = bsd.TargetLabel.val;
    }
  }

  public static BreakStmt FromDafny(Dafny.BreakStmt dafnyNode) {
    return new BreakStmt(dafnyNode);
  }

  public override Statement Clone() {
    return new BreakStmt(Count, TargetLabel);
  }
}