namespace AST;

public class MapDisplayExpr
: Expression, ConstructableFromDafny<Dafny.MapDisplayExpr, MapDisplayExpr> {
  private Type _Type;
  public override Type Type => _Type;

  public Dictionary<Expression, Expression> Items = new();

  public MapDisplayExpr(MapType t, Dictionary<Expression, Expression> items) {
    _Type = t;
    foreach (var (k, v) in items) {
      Items.Add(k, v);
    }
  }

  private MapDisplayExpr(Dafny.MapDisplayExpr mded) {
    _Type = Type.FromDafny(mded.Type);
    mded.Elements.ForEach(ep =>
      Items.Add(Expression.FromDafny(ep.A), Expression.FromDafny(ep.B)));
  }

  public static MapDisplayExpr FromDafny(Dafny.MapDisplayExpr dafnyNode) {
    return new MapDisplayExpr(dafnyNode);
  }
}