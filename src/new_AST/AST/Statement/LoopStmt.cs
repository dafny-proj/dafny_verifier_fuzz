namespace AST_new;

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
}

public partial class WhileLoopStmt : LoopStmt {
  public Expression? Guard { get; }

  public WhileLoopStmt(Expression? guard = null, BlockStmt? body = null,
  Specification? inv = null, Specification? mod = null, Specification? dec = null)
  : base(body, inv, mod, dec) {
    Guard = guard;
  }
}

public partial class ForLoopStmt : LoopStmt {
  public BoundVar LoopIndex { get; }
  public bool GoesUp { get; }
  public Expression LoopStart { get; }
  public Expression? LoopEnd { get; }

  public ForLoopStmt(BoundVar loopIndex, bool goesUp, Expression loopStart,
  Expression? loopEnd = null, BlockStmt? body = null, Specification? inv = null,
  Specification? mod = null, Specification? dec = null)
  : base(body, inv, mod, dec) {
    LoopIndex = loopIndex;
    GoesUp = goesUp;
    LoopStart = loopStart;
    LoopEnd = loopEnd;
  }
}
