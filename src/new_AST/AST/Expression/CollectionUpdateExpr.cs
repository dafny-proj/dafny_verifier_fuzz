namespace AST_new;

// Only for sequences, maps, multisets.
public partial class CollectionUpdateExpr : Expression {
  public Expression Collection { get; }
  public Expression Index { get; }
  public Expression Value { get; }

  public override Type Type => Collection.Type;

  public CollectionUpdateExpr(Expression collection,
  Expression index, Expression value) {
    Collection = collection;
    Index = index;
    Value = value;
  }

}
