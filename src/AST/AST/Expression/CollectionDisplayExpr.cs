namespace AST;

public abstract partial class CollectionDisplayExpr<T> : Expression { }
public partial class MapDisplayExpr : CollectionDisplayExpr<ExpressionPair> { }
public partial class SeqDisplayExpr : CollectionDisplayExpr<Expression> { }
public partial class SetDisplayExpr : CollectionDisplayExpr<Expression> { }
public partial class MultiSetDisplayExpr : CollectionDisplayExpr<Expression> { }

public abstract partial class CollectionDisplayExpr<T> : Expression
where T : Node {
  public List<T> Elements = new();

  public CollectionDisplayExpr(IEnumerable<T>? elements = null,
  Type? type = null) {
    if (elements != null) {
      Elements.AddRange(elements);
    }
    if (type != null) {
      Type = type;
    }
  }

  public override IEnumerable<Node> Children => Elements;
}

public partial class MapDisplayExpr
: CollectionDisplayExpr<ExpressionPair> {
  public MapDisplayExpr(IEnumerable<ExpressionPair>? elements = null,
  MapType? type = null) : base(elements, type) { }
}

public partial class SeqDisplayExpr : CollectionDisplayExpr<Expression> {
  public SeqDisplayExpr(IEnumerable<Expression>? elements = null,
  SeqType? type = null) : base(elements, type) { }
}

public partial class SetDisplayExpr : CollectionDisplayExpr<Expression> {
  public SetDisplayExpr(IEnumerable<Expression>? elements = null,
  SetType? type = null) : base(elements, type) { }
}

public partial class MultiSetDisplayExpr : CollectionDisplayExpr<Expression> {
  public MultiSetDisplayExpr(IEnumerable<Expression>? elements = null,
  MultiSetType? type = null) : base(elements, type) { }
}
