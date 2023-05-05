using Dafny = Microsoft.Dafny;

namespace Fuzzer;

public interface ILoopParser {
  public bool CanParseLoop(Node node);
  public ALoop ParseLoop(Node node);
}
public interface ILoopWriter {
  public bool CanWriteLoop(ALoop loop);
  public Node WriteLoop(ALoop loop);
}

public abstract class ALoop {
  public BlockStmt Body { get; set; }
  public List<AttributedExpression> Invariants { get; set; }
  public Specification<Dafny.FrameExpression, FrameExpression> Modifies { get; set; }
  public Specification<Dafny.Expression, Expression> Decreases { get; set; }

  public ALoop(BlockStmt body,
    List<AttributedExpression> inv,
    Specification<Dafny.FrameExpression, FrameExpression> mod,
    Specification<Dafny.Expression, Expression> dec
  ) {
    Body = body;
    Invariants = inv;
    Modifies = mod;
    Decreases = dec;
  }
}

/*  e.g. while condition {...} */
public class AConditionalLoop : ALoop {
  public Expression? Condition { get; set; }

  public AConditionalLoop(Expression? cond,
    BlockStmt body,
    List<AttributedExpression> inv,
    Specification<Dafny.FrameExpression, FrameExpression> mod,
    Specification<Dafny.Expression, Expression> dec)
  : base(body, inv, mod, dec) {
    Condition = cond;
  }
}

/*  e.g. for i := lo to hi {...} */
public class AIndexLoop : ALoop {
  public BoundVar Index { get; set; }
  public Expression IStart { get; set; }
  public Expression? IEnd { get; set; }
  public bool Up { get; set; }

  public AIndexLoop(BoundVar index, Expression start, Expression? end, bool up,
    BlockStmt body,
    List<AttributedExpression> inv,
    Specification<Dafny.FrameExpression, FrameExpression> mod,
    Specification<Dafny.Expression, Expression> dec)
  : base(body, inv, mod, dec) {
    Index = index;
    IStart = start;
    IEnd = end;
    Up = up;
  }
}
