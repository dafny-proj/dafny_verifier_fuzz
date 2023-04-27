namespace AST;

public class SeqSelectExpr
: Expression, ConstructableFromDafny<Dafny.SeqSelectExpr, SeqSelectExpr> {
  public override IEnumerable<Node> Children {
    get {
      yield return Seq;
      if (E0 != null) {
        yield return E0;
      }
      if (E1 != null) {
        yield return E1;
      }
    }
  }

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
  private Type _Type;
  public override Type Type { get => _Type; }

  private SeqSelectExpr(Dafny.SeqSelectExpr ssed) {
    Seq = Expression.FromDafny(ssed.Seq);
    E0 = ssed.E0 == null ? null : Expression.FromDafny(ssed.E0);
    E1 = ssed.E1 == null ? null : Expression.FromDafny(ssed.E1);
    SelectType = ssed.SelectOne ? SelectTypeT.Element : SelectTypeT.Slice;
    _Type = Type.FromDafny(ssed.Type);
  }

  public static SeqSelectExpr FromDafny(Dafny.SeqSelectExpr dafnyNode) {
    return new SeqSelectExpr(dafnyNode);
  }
}
