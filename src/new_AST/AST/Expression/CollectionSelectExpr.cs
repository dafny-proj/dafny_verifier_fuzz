namespace AST_new;

public abstract partial class CollectionSelectExpr : Expression { }
public partial class CollectionElementExpr : CollectionSelectExpr { }
public partial class CollectionSliceExpr : CollectionSelectExpr { }

public abstract partial class CollectionSelectExpr : Expression {
  public Expression Collection { get; }

  protected CollectionSelectExpr(Expression collection) {
    Collection = collection;
  }
}

public partial class CollectionElementExpr : CollectionSelectExpr {
  public Expression Index { get; }

  public CollectionElementExpr(Expression collection, Expression index)
  : base(collection) {
    Index = index;
  }
}

public partial class CollectionSliceExpr : CollectionSelectExpr {
  public Expression? Index0 { get; }
  public Expression? Index1 { get; }

  public CollectionSliceExpr(Expression collection,
  Expression? index0 = null, Expression? index1 = null)
  : base(collection) {
    Index0 = index0;
    Index1 = index1;
  }
}
