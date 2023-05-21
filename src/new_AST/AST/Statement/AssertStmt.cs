namespace AST_new;

public partial class AssertStmt : Statement {
  public Expression Assertion { get; }

  public AssertStmt(Expression assertion) {
    Assertion = assertion;
  }
}
