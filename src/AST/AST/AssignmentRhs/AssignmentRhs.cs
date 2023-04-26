namespace AST;

public abstract class AssignmentRhs
: Node, ConstructableFromDafny<Dafny.AssignmentRhs, AssignmentRhs> {
  public static AssignmentRhs FromDafny(Dafny.AssignmentRhs dafnyNode) {
    return dafnyNode switch {
      Dafny.ExprRhs exprRhsDafny => ExprRhs.FromDafny(exprRhsDafny),
      Dafny.TypeRhs typeRhsDafny => TypeRhs.FromDafny(typeRhsDafny),
      _ => throw new NotImplementedException(),
    };
  }
}
