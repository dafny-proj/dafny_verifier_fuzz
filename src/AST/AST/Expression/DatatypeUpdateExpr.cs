namespace AST;

public partial class DatatypeUpdateExpr : Expression {
  public Expression DatatypeValue { get; set; }
  public readonly List<DatatypeUpdatePair> Updates = new();

  public DatatypeUpdateExpr(Expression datatypeValue,
  IEnumerable<DatatypeUpdatePair> updates) {
    DatatypeValue = datatypeValue;
    Updates.AddRange(updates);
  }

  public override IEnumerable<Node> Children
    => Updates.Prepend<Node>(DatatypeValue);
  public override Type Type => DatatypeValue.Type;
}
