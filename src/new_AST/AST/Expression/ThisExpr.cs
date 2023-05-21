namespace AST_new;

public partial class ThisExpr : Expression { }
public partial class ImplicitThisExpr : ThisExpr { }

public partial class ThisExpr : Expression {
  public override Type Type { get; }

  public ThisExpr(Type type) {
    Type = type;
  }
}

public partial class ImplicitThisExpr : ThisExpr {
  public ImplicitThisExpr(Type type) : base(type) { }
}
