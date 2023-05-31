namespace AST;

public abstract partial class CollectionComprehensionExpr : Expression { }
public partial class SetComprehensionExpr : CollectionComprehensionExpr { }
public partial class MapComprehensionExpr : CollectionComprehensionExpr { }

public abstract partial class CollectionComprehensionExpr : Expression {
  public QuantifierDomain QuantifierDomain { get; }

  protected CollectionComprehensionExpr(QuantifierDomain quantifierDomain) {
    QuantifierDomain = quantifierDomain;
  }

  public override IEnumerable<Node> Children => new[] { QuantifierDomain };
}

public partial class SetComprehensionExpr : CollectionComprehensionExpr {
  public Expression? Value { get; set; }

  public SetComprehensionExpr(QuantifierDomain quantifierDomain,
  Expression? value) : base(quantifierDomain) {
    Value = value;
  }

  public override IEnumerable<Node> Children
    => Value == null ? base.Children : base.Children.Append(Value);
}

public partial class MapComprehensionExpr : CollectionComprehensionExpr {
  public Expression? Key { get; set; }
  public Expression Value { get; set; }

  public MapComprehensionExpr(QuantifierDomain quantifierDomain,
  Expression? key, Expression value) : base(quantifierDomain) {
    Key = key;
    Value = value;
  }

  public override IEnumerable<Node> Children {
    get {
      foreach (var c in base.Children) { yield return c; }
      if (Key != null) { yield return Key; }
      yield return Value;
    }
  }
}