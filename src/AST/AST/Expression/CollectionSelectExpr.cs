namespace AST;

public abstract partial class CollectionSelectExpr : Expression { }
public partial class CollectionElementExpr : CollectionSelectExpr { }
public partial class CollectionSliceExpr : CollectionSelectExpr { }

public abstract partial class CollectionSelectExpr : Expression {
  public Expression Collection { get; set; }

  protected CollectionSelectExpr(Expression collection, Type? type = null) {
    Collection = collection;
    if (type != null) { Type = type; }
  }

  public override IEnumerable<Node> Children => new[] { Collection };
}

// Only for sequences, multisets, maps, arrays. 
public partial class CollectionElementExpr : CollectionSelectExpr {
  public Expression Index { get; set; }

  public CollectionElementExpr(Expression collection, Expression index,
  Type? type = null)
  : base(collection, type) {
    Index = index;
  }

  public override IEnumerable<Node> Children => base.Children.Append(Index);
}

// Only for sequences, arrays.
public partial class CollectionSliceExpr : CollectionSelectExpr {
  public Expression? Index0 { get; set; }
  public Expression? Index1 { get; set; }

  public CollectionSliceExpr(Expression collection,
  Expression? index0 = null, Expression? index1 = null, Type? type = null)
  : base(collection, type) {
    Index0 = index0;
    Index1 = index1;
  }

  public override IEnumerable<Node> Children {
    get {
      foreach (var c in base.Children) { yield return c; }
      if (Index0 != null) { yield return Index0; }
      if (Index1 != null) { yield return Index1; }
    }
  }
}
