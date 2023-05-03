namespace AST;

public class WildcardExpr
: Expression, ConstructableFromDafny<Dafny.WildcardExpr, WildcardExpr> {
  private Type _Type;
  public override Type Type { get => _Type; }

  private WildcardExpr(Dafny.WildcardExpr wcd) {
    _Type = Type.FromDafny(wcd.Type);
  }

  public static WildcardExpr FromDafny(Dafny.WildcardExpr dafnyNode) {
    return new WildcardExpr(dafnyNode);
  }
}