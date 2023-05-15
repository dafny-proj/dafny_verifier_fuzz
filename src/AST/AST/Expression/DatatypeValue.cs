namespace AST;

public class DatatypeValue : Expression, ConstructableFromDafny<Dafny.DatatypeValue, DatatypeValue> {
  public override Type Type => throw new NotImplementedException();
  public DatatypeConstructor Constructor { get; }
  public List<Expression> Arguments = new();

  private DatatypeValue(Dafny.DatatypeValue dvd) {
    Constructor = DatatypeConstructor.FromDafny(dvd.Ctor);
    Arguments.AddRange(dvd.Arguments.Select(Expression.FromDafny));
  }

  public static DatatypeValue FromDafny(Dafny.DatatypeValue dafnyNode) {
    return new DatatypeValue(dafnyNode);
  }
}