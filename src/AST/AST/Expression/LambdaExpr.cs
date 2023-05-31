namespace AST;

public partial class LambdaExpr : Expression {
  public List<BoundVar> Params = new();
  public Expression Result { get; set; }
  public Specification? Precondition { get; }
  public Specification? ReadFrame { get; }

  public bool HasPrecondition()
    => Specification.HasUserDefinedSpec(Precondition);
  public bool HasReadFrame()
    => Specification.HasUserDefinedSpec(ReadFrame);

  public LambdaExpr(IEnumerable<BoundVar> params_, Expression result,
  Specification? pre = null, Specification? reads = null) {
    Params.AddRange(params_);
    Result = result;
    Precondition = pre;
    ReadFrame = reads;
  }

  public override IEnumerable<Node> Children {
    get {
      foreach (var p in Params) { yield return p; }
      if (HasPrecondition()) { yield return Precondition!; }
      if (HasReadFrame()) { yield return ReadFrame!; }
      yield return Result;
    }
  }
}
