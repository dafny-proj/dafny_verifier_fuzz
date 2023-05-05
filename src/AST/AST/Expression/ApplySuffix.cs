namespace AST;

public class ApplySuffix
: Expression, ConstructableFromDafny<Dafny.ApplySuffix, ApplySuffix> {
  public override IEnumerable<Node> Children => new Node[] { Lhs, ArgumentBindings };
  public Expression Lhs { get; set; }
  public ArgumentBindings ArgumentBindings { get; set; }
  public override Type Type {
    get => Lhs.Type;
  }

  public ApplySuffix(Expression lhs, ArgumentBindings args) {
    Lhs = lhs;
    ArgumentBindings = args;
  }

  private ApplySuffix(Dafny.ApplySuffix asd)
  : this(Expression.FromDafny(asd.Lhs),
         ArgumentBindings.FromDafny(asd.Bindings)) { }

  public static ApplySuffix FromDafny(Dafny.ApplySuffix dafnyNode) {
    return new ApplySuffix(dafnyNode);
  }

  public override Expression Clone() {
    return new ApplySuffix(Lhs.Clone(), ArgumentBindings.Clone());
  }
}