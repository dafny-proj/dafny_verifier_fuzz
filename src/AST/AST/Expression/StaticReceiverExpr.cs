namespace AST;

public partial class StaticReceiverExpr : Expression { }
public partial class ImplicitStaticReceiverExpr : StaticReceiverExpr { }

public partial class StaticReceiverExpr : Expression {
  public TopLevelDecl Decl { get; }

  public StaticReceiverExpr(TopLevelDecl decl) {
    Decl = decl;
  }

  public override IEnumerable<Node> Children => Enumerable.Empty<Node>();
}

public partial class ImplicitStaticReceiverExpr : StaticReceiverExpr {
  public ImplicitStaticReceiverExpr(TopLevelDecl decl) : base(decl) { }
}