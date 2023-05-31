namespace AST;

public abstract partial class TypeUnaryExpr : Expression { }
public partial class TypeConversionExpr : TypeUnaryExpr { }
public partial class TypeCheckExpr : TypeUnaryExpr { }

public abstract partial class TypeUnaryExpr : Expression {
  public Expression E { get; set; }
  public Type T { get; }
  public abstract string OpStr { get; }

  protected TypeUnaryExpr(Expression e, Type t) {
    E = e;
    T = t;
  }

  public override IEnumerable<Node> Children => new Node[] { E, T };
}

public partial class TypeConversionExpr : TypeUnaryExpr {
  public override string OpStr => "as";
  public TypeConversionExpr(Expression e, Type t) : base(e, t) { }
}

public partial class TypeCheckExpr : TypeUnaryExpr {
  public override string OpStr => "is";
  public TypeCheckExpr(Expression e, Type t) : base(e, t) { }
}
