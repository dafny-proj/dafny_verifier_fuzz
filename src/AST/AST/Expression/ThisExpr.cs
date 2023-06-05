namespace AST;

public partial class ThisExpr : Expression { }
public partial class ImplicitThisExpr : ThisExpr { }

public partial class ThisExpr : Expression {
  public ThisExpr(Type type) {
    Type = type;
  }

  public override IEnumerable<Node> Children => Enumerable.Empty<Node>();
}

public partial class ImplicitThisExpr : ThisExpr {
  public ImplicitThisExpr(Type type) : base(type) { }
}
