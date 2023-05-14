namespace AST;

public class NameSegment
: Expression, ConstructableFromDafny<Dafny.NameSegment, Expression> {
  public string Name { get; set; }
  private Type _Type;
  public override Type Type { get => _Type; }

  public NameSegment(string name, Type type) {
    Name = name;
    _Type = type;
  }

  private NameSegment(Dafny.NameSegment nameSegmentDafny) {
    Name = nameSegmentDafny.Name;
    _Type = Type.FromDafny(nameSegmentDafny.Type);
  }

  // TODO: referring to the Dafny documentation, all ConcreteSyntaxExpressions
  // are replaced by ResolvedExpression after resolution.
  public static Expression FromDafny(Dafny.NameSegment dafnyNode) {
    if (dafnyNode.ResolvedExpression != null) {
      return Expression.FromDafny(dafnyNode.ResolvedExpression);
    }
    return new NameSegment(dafnyNode);
  }

  public override Expression Clone() {
    // TODO: clone type?
    return new NameSegment(Name, Type);
  }
}
