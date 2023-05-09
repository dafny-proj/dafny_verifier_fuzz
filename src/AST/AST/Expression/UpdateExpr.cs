namespace AST;

// Only for updatable collections: seq, map, multiset.
// collection[index := value]
public class CollectionUpdateExpr
: Expression, ConstructableFromDafny<Dafny.SeqUpdateExpr, CollectionUpdateExpr> {
  public override Type Type => Collection.Type;

  public Expression Collection { get; set; }
  public Expression Index { get; set; }
  public Expression Value { get; set; }

  public CollectionUpdateExpr(Expression collection, Expression index, Expression value) {
    Collection = collection;
    Index = index;
    Value = value;
  }

  private CollectionUpdateExpr(Dafny.SeqUpdateExpr sued)
  : this(Expression.FromDafny(sued.Seq),
    Expression.FromDafny(sued.Index),
    Expression.FromDafny(sued.Value)) { }

  public static CollectionUpdateExpr FromDafny(Dafny.SeqUpdateExpr dafnyNode) {
    return new CollectionUpdateExpr(dafnyNode);
  }

  public override IEnumerable<Node> Children => new[] { Collection, Index, Value };
  public override Expression Clone() => new CollectionUpdateExpr(
    Collection.Clone(), Index.Clone(), Value.Clone());
}