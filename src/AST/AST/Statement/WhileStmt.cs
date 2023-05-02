namespace AST;

public class WhileStmt
: Statement, ConstructableFromDafny<Dafny.WhileStmt, WhileStmt> {
  public override IEnumerable<Node> Children {
    get {
      if (Guard != null) {
        yield return Guard;
      }
      if (Body != null) {
        yield return Body;
      }
      foreach (var i in Invariants) {
        yield return i;
      }
      yield return Modifies;
      yield return AllDecreases;
    }
  }
  public Expression? Guard { get; set; }
  public BlockStmt? Body { get; set; }
  public List<AttributedExpression> Invariants = new List<AttributedExpression>();
  public Specification<Dafny.FrameExpression, FrameExpression> Modifies { get; set; }
  private Specification<Dafny.Expression, Expression> _Decreases { get; set; }
  public Specification<Dafny.Expression, Expression> AllDecreases {
    get => _Decreases;
  }
  public Specification<Dafny.Expression, Expression> ProvidedDecreases {
    get => _Decreases.GetProvided();
  }

  public WhileStmt(Expression? guard,
    BlockStmt? body,
    List<AttributedExpression> inv,
    Specification<Dafny.FrameExpression, FrameExpression> mod,
    Specification<Dafny.Expression, Expression> dec) {
    Guard = guard;
    Body = body;
    Invariants.AddRange(inv);
    Modifies = mod;
    _Decreases = dec;
  }

  private WhileStmt(Dafny.WhileStmt wsd) {
    Guard = wsd.Guard == null ? null : Expression.FromDafny(wsd.Guard);
    Body = wsd.Body == null ? null : BlockStmt.FromDafny(wsd.Body);
    Invariants.AddRange(wsd.Invariants.Select(AttributedExpression.FromDafny));
    Modifies = Specification<Dafny.FrameExpression, FrameExpression>.FromDafny(wsd.Mod);
    _Decreases = Specification<Dafny.Expression, Expression>.FromDafny(wsd.Decreases);
  }

  public static WhileStmt FromDafny(Dafny.WhileStmt dafnyNode) {
    return new WhileStmt(dafnyNode);
  }
}
