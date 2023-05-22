namespace AST_new;

public partial class PrintStmt : Statement {
  public List<Expression> Expressions = new();

  public PrintStmt(IEnumerable<Expression> expressions) {
    Expressions.AddRange(expressions);
  }

  public override IEnumerable<Node> Children => Expressions;

}
