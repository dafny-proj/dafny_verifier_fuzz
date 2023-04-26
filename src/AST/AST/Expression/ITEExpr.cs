namespace AST;

public class ITEExpr
: Expression, ConstructableFromDafny<Dafny.ITEExpr, ITEExpr> {
  public Expression Guard { get; set; }
  public Expression Thn { get; set; }
  public Expression Els { get; set; }

  private ITEExpr(Dafny.ITEExpr iteed) {
    Guard = Expression.FromDafny(iteed.Test);
    Thn = Expression.FromDafny(iteed.Thn);
    Els = Expression.FromDafny(iteed.Els);
  }

  public static ITEExpr FromDafny(Dafny.ITEExpr dafnyNode) {
    return new ITEExpr(dafnyNode);
  }
}