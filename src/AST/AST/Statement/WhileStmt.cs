namespace AST;

public class WhileStmt
: Statement, ConstructableFromDafny<Dafny.WhileStmt, WhileStmt> {
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
