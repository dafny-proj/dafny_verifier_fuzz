namespace AST;

public class ApplySuffix
: Expression, ConstructableFromDafny<Dafny.ApplySuffix, ApplySuffix> {
  public override IEnumerable<Node> Children => new Node[] { Lhs, ArgumentBindings };
  public Expression Lhs { get; set; }
  public ArgumentBindings ArgumentBindings { get; set; }
  public override Type Type {
    get => Lhs.Type;
  }

  private ApplySuffix(Dafny.ApplySuffix asd) {
    Lhs = Expression.FromDafny(asd.Lhs);
    ArgumentBindings = ArgumentBindings.FromDafny(asd.Bindings);
  }

  public static ApplySuffix FromDafny(Dafny.ApplySuffix dafnyNode) {
    return new ApplySuffix(dafnyNode);
  }
}