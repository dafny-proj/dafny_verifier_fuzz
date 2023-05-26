using System.Diagnostics.Contracts;

namespace AST_new;

public partial class FunctionCallExpr : Expression {
  public MemberSelectExpr Callee { get; set; }
  public readonly List<Expression> Arguments = new();

  public FunctionCallExpr(MemberSelectExpr callee,
  IEnumerable<Expression>? arguments = null) {
    Contract.Requires(callee.Member is FunctionDecl);
    Callee = callee;
    if (arguments != null) {
      Arguments.AddRange(arguments);
    }
  }

  public override IEnumerable<Node> Children
    => new[] { Callee }.Concat<Node>(Arguments);
}
