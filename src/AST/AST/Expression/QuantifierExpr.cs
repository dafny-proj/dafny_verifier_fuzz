namespace AST;

public abstract partial class QuantifierExpr : Expression { }
public partial class ForallExpr : QuantifierExpr { }
public partial class ExistsExpr : QuantifierExpr { }

// (forall | exists) QuantifierDomain :: Term
public abstract partial class QuantifierExpr : Expression {
  public string Quantifier { get; }
  public QuantifierDomain QuantifierDomain { get; set; }
  public Expression Term { get; set; }

  protected QuantifierExpr(string quantifier, QuantifierDomain quantifierDomain,
  Expression term) {
    Quantifier = quantifier;
    QuantifierDomain = quantifierDomain;
    Term = term;
  }

  public override IEnumerable<Node> Children
    => new Node[] { QuantifierDomain, Term };
  public override Type Type => Type.Bool;
}

public partial class ForallExpr : QuantifierExpr {
  public ForallExpr(QuantifierDomain quantifierDomain, Expression term)
  : base("forall", quantifierDomain, term) { }
}

public partial class ExistsExpr : QuantifierExpr {
  public ExistsExpr(QuantifierDomain quantifierDomain, Expression term)
  : base("exists", quantifierDomain, term) { }
}
