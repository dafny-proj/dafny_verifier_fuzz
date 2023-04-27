namespace AST;

public class ITEExpr
: Expression, ConstructableFromDafny<Dafny.ITEExpr, ITEExpr> {
  public override IEnumerable<Node> Children
    => new Node[] { Guard, Thn, Els };

  public Expression Guard { get; set; }
  public Expression Thn { get; set; }
  public Expression Els { get; set; }
  protected Type _Type;
  public override Type Type { get => _Type; }

  private ITEExpr(Dafny.ITEExpr iteed) {
    Guard = Expression.FromDafny(iteed.Test);
    Thn = Expression.FromDafny(iteed.Thn);
    Els = Expression.FromDafny(iteed.Els);
    _Type = Type.FromDafny(iteed.Type);
  }

  public static ITEExpr FromDafny(Dafny.ITEExpr dafnyNode) {
    return new ITEExpr(dafnyNode);
  }
}