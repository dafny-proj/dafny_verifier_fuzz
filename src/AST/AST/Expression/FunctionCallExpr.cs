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
    TrySetType();
  }

  private void TrySetType() {
    // FIXME: Generics and propagation of type arguments are handled incorrectly.
    if (Callee.Type is CallableType ct) {
      if (ct.Callable is FunctionDecl fd) {
        Type = fd.ResultType;
      }
    } else if (Callee.Type is ArrowType at) {
      Type = at.ResultType;
    } else {
      Type = new TypeProxy();
    }
  }

  public override IEnumerable<Node> Children
    => new[] { Callee }.Concat<Node>(Arguments);
}
