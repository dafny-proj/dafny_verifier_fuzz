namespace AST;

public partial class MultiSetConstructionExpr : Expression { }
public partial class SeqConstructionExpr : Expression { }

// Construct multiset from a set/sequence.
public partial class MultiSetConstructionExpr : Expression {
  public Expression E;

  public MultiSetConstructionExpr(Expression e) {
    E = e;
  }

  public override IEnumerable<Node> Children => new[] { E };
}

// Construct a sequence via an initialising function.
// e.g. seq(3, i => i) makes [0, 1, 2]
public partial class SeqConstructionExpr : Expression {
  public Expression Count { get; set; }
  public Expression Initialiser { get; set; }

  public SeqConstructionExpr(Expression count, Expression initialiser) {
    Count = count;
    Initialiser = initialiser;
  }

  public override IEnumerable<Node> Children => new[] { Count, Initialiser };
}
