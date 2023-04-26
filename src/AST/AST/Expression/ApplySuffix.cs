namespace AST;

public class ApplySuffix
: Expression, ConstructableFromDafny<Dafny.ApplySuffix, ApplySuffix> {
  public Expression Lhs { get; set; }
  public ArgumentBindings ArgumentBindings { get; set; }

  private ApplySuffix(Dafny.ApplySuffix asd) {
    Lhs = Expression.FromDafny(asd.Lhs);
    ArgumentBindings = ArgumentBindings.FromDafny(asd.Bindings);
  }

  public static ApplySuffix FromDafny(Dafny.ApplySuffix dafnyNode) {
    return new ApplySuffix(dafnyNode);
  }
}