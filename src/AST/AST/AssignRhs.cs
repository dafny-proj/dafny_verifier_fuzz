namespace AST;

public abstract class AssignmentRhs
: Node, ConstructableFromDafny<Dafny.AssignmentRhs, AssignmentRhs> {
  public static AssignmentRhs FromDafny(Dafny.AssignmentRhs dafnyNode) {
    return dafnyNode switch {
      Dafny.ExprRhs exprRhsDafny => ExprRhs.FromDafny(exprRhsDafny),
      _ => throw new NotImplementedException(),
    };
  }
}

public class ExprRhs
: AssignmentRhs, ConstructableFromDafny<Dafny.ExprRhs, ExprRhs> {
  public Expression Expr;
  private ExprRhs(Dafny.ExprRhs exprRhsDafny) {
    Expr = Expression.FromDafny(exprRhsDafny.Expr);
  }
  public static ExprRhs FromDafny(Dafny.ExprRhs dafnyNode) {
    return new ExprRhs(dafnyNode);
  }
}