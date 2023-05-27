namespace AST.Cloner;

public partial class ASTCloner {
  private Variable CreateVariableClone(Variable v) {
    var name = v.Name;
    var type = CloneType(v.Type);
    var explicitType = v.HasExplicitType() ? CloneType(v.ExplicitType!) : null;
    return v switch {
      LocalVar lv => new LocalVar(name, type, explicitType),
      BoundVar bv => new BoundVar(name, type, explicitType),
      Formal fv => new Formal(name, type, defaultValue:
        fv.DefaultValue == null ? null : CloneExpression(fv.DefaultValue)),
      _ => throw new UnsupportedNodeCloningException(v),
    };
  }

  private Variable CloneVariable(Variable v) {
    return GetOrCreateVariableClone(v);
  }

  private BoundVar CloneBoundVar(BoundVar v) {
    return (BoundVar)GetOrCreateVariableClone(v);
  }

  private LocalVar CloneLocalVar(LocalVar v) {
    return (LocalVar)GetOrCreateVariableClone(v);
  }

  private Formal CloneFormal(Formal v) {
    return (Formal)GetOrCreateVariableClone(v);
  }

  private Specification? CloneSpecification(Specification? s) {
    if (s == null) { return null; }
    return new Specification(
      s.SpecificationType, s.Expressions.Select(CloneExpression));
  }

  private ExpressionPair CloneExpressionPair(ExpressionPair ep) {
    return new ExpressionPair(
      CloneExpression(ep.Key), CloneExpression(ep.Value));
  }

  private AssignmentPair CloneAssignmentPair(AssignmentPair ap) {
    return new AssignmentPair(
      CloneExpression(ap.Key), CloneAssignmentRhs(ap.Value));
  }
}
