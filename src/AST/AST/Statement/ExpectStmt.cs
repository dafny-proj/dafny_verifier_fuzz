namespace AST;

public partial class ExpectStmt : Statement {
  public Expression Expectation { get; set; }

  public ExpectStmt(Expression expectation) {
    Expectation = expectation;
  }

  public override IEnumerable<Node> Children => new[] { Expectation };
}
