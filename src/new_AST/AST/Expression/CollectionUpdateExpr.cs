namespace AST_new;

// Only for sequences, maps, multisets.
public partial class CollectionUpdateExpr : Expression {
  public Expression Collection { get; set; }
  public Expression Index { get; set; }
  public Expression Value { get; set; }

  public override Type Type => Collection.Type;

  public CollectionUpdateExpr(Expression collection,
  Expression index, Expression value) {
    Collection = collection;
    Index = index;
    Value = value;
  }

  public override IEnumerable<Node> Children
    => new[] { Collection, Index, Value };
}
