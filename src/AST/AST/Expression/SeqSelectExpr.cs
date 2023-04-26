namespace AST;

public class SeqSelectExpr
: Expression, ConstructableFromDafny<Dafny.SeqSelectExpr, SeqSelectExpr> {

  public enum SelectTypeT {
    Element,
    Slice,
  }

  public Expression Seq { get; set; }
  public Expression? E0 { get; }
  public Expression? E1 { get; }
  public SelectTypeT SelectType { get; }

  public bool IsElement => SelectType == SelectTypeT.Element;
  public bool IsSlice => SelectType == SelectTypeT.Slice;

  private SeqSelectExpr(Dafny.SeqSelectExpr ssed) {
    Seq = Expression.FromDafny(ssed.Seq);
    E0 = ssed.E0 == null ? null : Expression.FromDafny(ssed.E0);
    E1 = ssed.E1 == null ? null : Expression.FromDafny(ssed.E1);
    SelectType = ssed.SelectOne ? SelectTypeT.Element : SelectTypeT.Slice;
  }

  public static SeqSelectExpr FromDafny(Dafny.SeqSelectExpr dafnyNode) {
    return new SeqSelectExpr(dafnyNode);
  }
}
