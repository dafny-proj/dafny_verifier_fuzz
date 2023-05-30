namespace AST;

// Convert a set/sequence to a multiset.
public partial class MultiSetFormingExpr : Expression {
  public Expression E;

  public MultiSetFormingExpr(Expression e) {
    E = e;
  }

  public override IEnumerable<Node> Children => new[] { E };
}
