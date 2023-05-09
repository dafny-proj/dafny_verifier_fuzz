namespace AST;

public class CollectionSelectExpr
: Expression, ConstructableFromDafny<Dafny.SeqSelectExpr, CollectionSelectExpr> {
  public override IEnumerable<Node> Children {
    get {
      yield return Collection;
      if (Index0 != null) {
        yield return Index0;
      }
      if (Index1 != null) {
        yield return Index1;
      }
    }
  }

  public enum SelectTypeT {
    Element,
    Slice,
  }

  public Expression Collection { get; set; }
  public Expression? Index0 { get; }
  public Expression? Index1 { get; }
  public SelectTypeT SelectType { get; }

  public bool IsElement => SelectType == SelectTypeT.Element;
  public bool IsSlice => SelectType == SelectTypeT.Slice;
  private Type _Type;
  public override Type Type { get => _Type; }

  private CollectionSelectExpr(Dafny.SeqSelectExpr ssed) {
    Collection = Expression.FromDafny(ssed.Seq);
    Index0 = ssed.E0 == null ? null : Expression.FromDafny(ssed.E0);
    Index1 = ssed.E1 == null ? null : Expression.FromDafny(ssed.E1);
    SelectType = ssed.SelectOne ? SelectTypeT.Element : SelectTypeT.Slice;
    _Type = Type.FromDafny(ssed.Type);
  }

  public static CollectionSelectExpr FromDafny(Dafny.SeqSelectExpr dafnyNode) {
    return new CollectionSelectExpr(dafnyNode);
  }
}
