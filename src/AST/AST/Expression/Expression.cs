namespace AST;

public abstract class Expression
: Node, ConstructableFromDafny<Dafny.Expression, Expression> {
  public static Expression FromDafny(Dafny.Expression dafnyNode) {
    return dafnyNode switch {
      Dafny.NameSegment nameSeg
        => NameSegment.FromDafny(nameSeg),
      Dafny.BinaryExpr binExpr
        => BinaryExpr.FromDafny(binExpr),
      Dafny.LiteralExpr litExpr
        => LiteralExpr.FromDafny(litExpr),
      Dafny.ParensExpression parensExpr
        => ParensExpression.FromDafny(parensExpr),
      Dafny.NegationExpression negExpr
        => NegationExpression.FromDafny(negExpr),
      Dafny.IdentifierExpr identExpr
        => IdentifierExpr.FromDafny(identExpr),
      Dafny.ApplySuffix applySuffix
        => ApplySuffix.FromDafny(applySuffix),
      Dafny.ITEExpr itee
        => ITEExpr.FromDafny(itee),
      Dafny.ChainingExpression ce
        => ChainingExpression.FromDafny(ce),
      Dafny.SeqSelectExpr sse
        => SeqSelectExpr.FromDafny(sse),
      _ => throw new NotImplementedException($"{dafnyNode.GetType()}"),
    };
  }
}