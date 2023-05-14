namespace AST;

public class MapDisplayExprElement : Node {
  public Expression Index { get; set; }
  public Expression Value { get; set; }
  public MapDisplayExprElement(Expression index, Expression value) {
    Index = index;
    Value = value;
  }
}

public class MapDisplayExpr
: Expression, ConstructableFromDafny<Dafny.MapDisplayExpr, MapDisplayExpr> {
  private Type _Type;
  public override Type Type => _Type;

  // TODO: Dict may not preserve order.
  public List<MapDisplayExprElement> Elements = new();

  public MapDisplayExpr(MapType t, List<MapDisplayExprElement>? elements = null) {
    _Type = t;
    if (elements != null) {
      Elements.AddRange(elements);
    }
  }

  private MapDisplayExpr(Dafny.MapDisplayExpr mded) {
    _Type = Type.FromDafny(mded.Type);
    mded.Elements.ForEach(ep =>
      Elements.Add(new MapDisplayExprElement(
        Expression.FromDafny(ep.A), Expression.FromDafny(ep.B))));
  }

  public static MapDisplayExpr FromDafny(Dafny.MapDisplayExpr dafnyNode) {
    return new MapDisplayExpr(dafnyNode);
  }
}