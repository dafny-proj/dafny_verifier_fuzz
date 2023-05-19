namespace AST_new;

public abstract partial class CollectionDisplayExpr<T> : Expression { }
public partial class MapDisplayExpr : CollectionDisplayExpr<ExpressionPair> { }
public partial class SeqDisplayExpr : CollectionDisplayExpr<Expression> { }
public partial class SetDisplayExpr : CollectionDisplayExpr<Expression> { }
public partial class MultiSetDisplayExpr : CollectionDisplayExpr<Expression> { }

public abstract partial class CollectionDisplayExpr<T> : Expression {
  public List<T> Elements = new();

  public CollectionDisplayExpr(IEnumerable<T>? elements) {
    if (elements != null) {
      Elements.AddRange(elements);
    }
  }
}

public partial class MapDisplayExpr
: CollectionDisplayExpr<ExpressionPair> {
  public MapDisplayExpr(IEnumerable<ExpressionPair>? elements)
  : base(elements) { }
}

public partial class SeqDisplayExpr : CollectionDisplayExpr<Expression> {
  public SeqDisplayExpr(IEnumerable<Expression>? elements)
  : base(elements) { }
}

public partial class SetDisplayExpr : CollectionDisplayExpr<Expression> {
  public SetDisplayExpr(IEnumerable<Expression>? elements)
  : base(elements) { }
}

public partial class MultiSetDisplayExpr : CollectionDisplayExpr<Expression> {
  public MultiSetDisplayExpr(IEnumerable<Expression>? elements)
  : base(elements) { }
}
