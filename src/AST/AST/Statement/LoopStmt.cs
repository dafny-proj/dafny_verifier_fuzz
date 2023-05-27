namespace AST;

public abstract partial class LoopStmt : Statement { }
public partial class WhileLoopStmt : LoopStmt { }
public partial class ForLoopStmt : LoopStmt { }

public abstract partial class LoopStmt : Statement {
  public BlockStmt? Body { get; }
  public Specification? Invariants { get; protected set; }
  public Specification? Modifies { get; protected set; }
  public Specification? Decreases { get; protected set; }

  public bool HasBody() => Body != null;
  public bool HasInvariants() => Specification.HasUserDefinedSpec(Invariants);
  public bool HasModifiesSpec() => Specification.HasUserDefinedSpec(Modifies);
  public bool HasDecreasesSpec() => Specification.HasUserDefinedSpec(Decreases);
  public bool HasSpec()
    => HasInvariants() || HasModifiesSpec() || HasDecreasesSpec();

  public LoopStmt(BlockStmt? body = null, Specification? inv = null,
  Specification? mod = null, Specification? dec = null) {
    Body = body;
    Invariants = inv;
    Modifies = mod;
    Decreases = dec;
  }

  public override IEnumerable<Node> Children {
    get {
      if (HasInvariants()) { yield return Invariants!; }
      if (HasModifiesSpec()) { yield return Modifies!; }
      if (HasDecreasesSpec()) { yield return Decreases!; }
      if (Body != null) { yield return Body; }
    }
  }
}

public partial class WhileLoopStmt : LoopStmt {
  public Expression? Guard { get; set; }

  public WhileLoopStmt(Expression? guard = null, BlockStmt? body = null,
  Specification? inv = null, Specification? mod = null, Specification? dec = null)
  : base(body, inv, mod, dec) {
    Guard = guard;
  }

  public override IEnumerable<Node> Children
    => Guard == null ? base.Children : base.Children.Prepend(Guard);
}

public partial class ForLoopStmt : LoopStmt {
  public BoundVar LoopIndex { get; }
  public bool GoesUp { get; }
  public Expression LoopStart { get; set; }
  public Expression? LoopEnd { get; set; }

  public ForLoopStmt(BoundVar loopIndex, bool goesUp, Expression loopStart,
  Expression? loopEnd = null, BlockStmt? body = null, Specification? inv = null,
  Specification? mod = null, Specification? dec = null)
  : base(body, inv, mod, dec) {
    LoopIndex = loopIndex;
    GoesUp = goesUp;
    LoopStart = loopStart;
    LoopEnd = loopEnd;
  }

  public override IEnumerable<Node> Children {
    get {
      yield return LoopIndex;
      yield return LoopStart;
      if (LoopEnd != null) { yield return LoopEnd; }
      foreach (var c in base.Children) { yield return c; }
    }
  }
}
