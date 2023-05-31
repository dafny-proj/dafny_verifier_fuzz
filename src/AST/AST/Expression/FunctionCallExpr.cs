using System.Diagnostics.Contracts;

namespace AST;

public partial class FunctionCallExpr : Expression {
  public Expression Callee { get; set; }
  public readonly List<Expression> Arguments = new();

  public FunctionCallExpr(Expression callee,
  IEnumerable<Expression>? arguments = null) {
    Callee = callee;
    if (arguments != null) {
      Arguments.AddRange(arguments);
    }
  }

  public override IEnumerable<Node> Children
    => new[] { Callee }.Concat<Node>(Arguments);
}
