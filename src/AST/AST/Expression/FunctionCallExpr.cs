using System.Diagnostics.Contracts;

namespace AST;

public partial class FunctionCallExpr : Expression {
  public Expression Callee { get; }
  public readonly List<Expression> Arguments = new();

  public FunctionCallExpr(Expression callee,
  IEnumerable<Expression>? arguments = null) {
    Callee = callee;
    if (arguments != null) {
      Arguments.AddRange(arguments);
    }
    Type = ((FunctionDecl)((MemberSelectExpr)callee).Member).ResultType;
  }

  public override IEnumerable<Node> Children
    => new[] { Callee }.Concat<Node>(Arguments);
}
