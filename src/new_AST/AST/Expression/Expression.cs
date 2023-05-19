namespace AST_new;

public abstract partial class Expression : Node {
  public virtual Type Type {
    get => throw new UnsupportedASTOperationException(this,
      "expression type retrieval");
  }
}

public partial class ParensExpr : Expression {
  public Expression E { get; }
  public override Type Type => E.Type;

  public ParensExpr(Expression e) {
    E = e;
  }
}
