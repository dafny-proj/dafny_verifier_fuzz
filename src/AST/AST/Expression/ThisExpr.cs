namespace AST;

public class ThisExpr
: Expression, ConstructableFromDafny<Dafny.ThisExpr, ThisExpr> {
  private Type _Type;
  public override Type Type => _Type;

  public ThisExpr(Type type) {
    _Type = type;
  }

  private ThisExpr(Dafny.ThisExpr ted)
  : this(Type.FromDafny(ted.Type)) { }

  public static ThisExpr FromDafny(Dafny.ThisExpr dafnyNode) {
    return new ThisExpr(dafnyNode);
  }
}