using Dafny = Microsoft.Dafny;

namespace Fuzzer;

public interface LoopParser {
  public bool CanParseLoop(Node node);
  public Loop ParseLoop(Node node);
}
public interface LoopWriter {
  public bool CanWriteLoop(Loop loop);
  public Node WriteLoop(Loop loop);
}

public class Loop {
  public LoopGuard Guard { get; set; }
  public LoopBody Body { get; set; }
  public LoopSpec Spec { get; set; }

  public Loop(LoopGuard guard, LoopBody body, LoopSpec spec) {
    Guard = guard;
    Body = body;
    Spec = spec;
  }
}

public abstract class LoopGuard { }
public class NoLoopGuard : LoopGuard {
  /*  e.g. while * {...} */
}
public class ConditionalLoopGuard : LoopGuard {
  /*  e.g. while condition {...} */
  public Expression Condition { get; set; }
  public ConditionalLoopGuard(Expression condition) {
    Condition = condition;
  }
}
public class IndexLoopGuard : LoopGuard {
  /*  e.g. for i := lo to hi {...} */
  Expression? Lo { get; set; }
  Expression? Hi { get; set; }
  public IndexLoopGuard(Expression? Lo, Expression? Hi) { }
}

public abstract class LoopBody {}
public class NoLoopBody : LoopBody { }
public class BlockLoopBody : LoopBody {
  public BlockStmt Block { get; set; }
  public BlockLoopBody(BlockStmt block) {
    Block = block;
  }
}

// TODO: single loop spec or separate inv, dec, mod?
public class LoopSpec {
  public List<AttributedExpression> Invariants { get; set; }
  public Specification<Dafny.FrameExpression, FrameExpression> Modifies { get; set; }
  public Specification<Dafny.Expression, Expression> Decreases { get; set; }

  public LoopSpec(List<AttributedExpression> inv,
  Specification<Dafny.FrameExpression, FrameExpression> mod,
  Specification<Dafny.Expression, Expression> dec) { 
    // TODO: clone instead of reference?
    Invariants = inv;
    Modifies = mod;
    Decreases = dec;
  }
}