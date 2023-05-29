namespace AST;

public partial class ForallStmt : Statement {
  public QuantifierDomain QuantifierDomain { get; }
  public Specification? Ensures { get; }
  public Statement? Body { get; }

  public ForallStmt(QuantifierDomain quantifierDomain,
  Specification? ensures = null, Statement? body = null) {
    QuantifierDomain = quantifierDomain;
    Ensures = ensures;
    Body = body;
  }

  public override IEnumerable<Node> Children {
    get {
      yield return QuantifierDomain;
      if (Ensures != null) { yield return Ensures; }
      if (Body != null) { yield return Body; }
    }
  }
}
