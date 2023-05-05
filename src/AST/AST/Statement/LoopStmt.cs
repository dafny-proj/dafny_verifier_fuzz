namespace AST;

public abstract class LoopStmt
: Statement, ConstructableFromDafny<Dafny.LoopStmt, LoopStmt> {
  public List<AttributedExpression> Invariants = new List<AttributedExpression>();
  public Specification<Dafny.FrameExpression, FrameExpression> Modifies { get; set; }
  protected Specification<Dafny.Expression, Expression> _Decreases { get; set; }
  public Specification<Dafny.Expression, Expression> AllDecreases {
    get => _Decreases;
  }
  public Specification<Dafny.Expression, Expression> ProvidedDecreases {
    get => _Decreases.GetProvided();
  }

  protected LoopStmt(Dafny.LoopStmt lsd) : this(
    /*inv=*/lsd.Invariants.Select(AttributedExpression.FromDafny).ToList(),
    /*mod=*/Specification<Dafny.FrameExpression, FrameExpression>.FromDafny(lsd.Mod),
    /*dec=*/Specification<Dafny.Expression, Expression>.FromDafny(lsd.Decreases)
  ) { }

  protected LoopStmt(IEnumerable<AttributedExpression> inv,
    Specification<Dafny.FrameExpression, FrameExpression> mod,
    Specification<Dafny.Expression, Expression> dec) {
    Invariants.AddRange(inv);
    Modifies = mod;
    _Decreases = dec;
  }

  public static LoopStmt FromDafny(Dafny.LoopStmt dafnyNode) {
    return dafnyNode switch {
      Dafny.WhileStmt ws => WhileStmt.FromDafny(ws),
      Dafny.ForLoopStmt fs => ForLoopStmt.FromDafny(fs),
      _ => throw new NotImplementedException($"Unhandled translation from Dafny for `{dafnyNode.GetType()}`"),
    };
  }

  public override IEnumerable<Node> Children {
    get {
      foreach (var i in Invariants) {
        yield return i;
      }
      yield return Modifies;
      yield return AllDecreases;
    }
  }
}

public class WhileStmt
: LoopStmt, ConstructableFromDafny<Dafny.WhileStmt, WhileStmt> {
  public Expression? Guard { get; set; }
  public BlockStmt? Body { get; set; }

  public WhileStmt(Expression? guard,
    BlockStmt? body,
    IEnumerable<AttributedExpression> inv,
    Specification<Dafny.FrameExpression, FrameExpression> mod,
    Specification<Dafny.Expression, Expression> dec)
    : base(inv, mod, dec) {
    Guard = guard;
    Body = body;
  }

  private WhileStmt(Dafny.WhileStmt wsd)
  : base(wsd) {
    Guard = wsd.Guard == null ? null : Expression.FromDafny(wsd.Guard);
    Body = wsd.Body == null ? null : BlockStmt.FromDafny(wsd.Body);
  }

  public static WhileStmt FromDafny(Dafny.WhileStmt dafnyNode) {
    return new WhileStmt(dafnyNode);
  }

  public override IEnumerable<Node> Children {
    get {
      if (Guard != null) {
        yield return Guard;
      }
      if (Body != null) {
        yield return Body;
      }
      foreach (var c in base.Children) {
        yield return c;
      }
    }
  }

  public override Statement Clone() {
    return new WhileStmt(
      Guard?.Clone(),
      Body?.Clone(),
      Invariants.Select(i => i.Clone()),
      Modifies.Clone(),
      AllDecreases.Clone()
    );
  }
}

public class ForLoopStmt :
LoopStmt, ConstructableFromDafny<Dafny.ForLoopStmt, ForLoopStmt> {
  public BoundVar LoopIndex { get; set; }
  public Expression Start { get; set; }
  public Expression? End { get; set; }
  public bool GoingUp { get; set; }
  public BlockStmt? Body { get; set; }

  public ForLoopStmt(BoundVar index, Expression start, Expression? end, bool up,
    BlockStmt? body,
    IEnumerable<AttributedExpression> inv,
    Specification<Dafny.FrameExpression, FrameExpression> mod,
    Specification<Dafny.Expression, Expression> dec) : base(inv, mod, dec) {
    LoopIndex = index;
    Start = start;
    End = end;
    GoingUp = up;
    Body = body;
  }

  private ForLoopStmt(Dafny.ForLoopStmt fsd) : base(fsd) {
    LoopIndex = BoundVar.FromDafny(fsd.LoopIndex);
    Start = Expression.FromDafny(fsd.Start);
    End = fsd.End == null ? null : Expression.FromDafny(fsd.End);
    GoingUp = fsd.GoingUp;
    Body = fsd.Body == null ? null : BlockStmt.FromDafny(fsd.Body);
  }

  public static ForLoopStmt FromDafny(Dafny.ForLoopStmt dafnyNode) {
    return new ForLoopStmt(dafnyNode);
  }

  public override IEnumerable<Node> Children {
    get {
      yield return Start;
      if (End != null) {
        yield return End;
      }
      if (Body != null) {
        yield return Body;
      }
      foreach (var c in base.Children) {
        yield return c;
      }
    }
  }
}
