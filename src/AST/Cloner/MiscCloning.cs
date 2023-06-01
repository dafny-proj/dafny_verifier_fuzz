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

  private QuantifierDomain CloneQuantifierDomain(QuantifierDomain qd) {
    return new QuantifierDomain(qd.Vars.Select(CloneBoundVar),
      qd.Range == null ? null : CloneExpression(qd.Range));
  }

  private MatchExprCase CloneMatchExprCase(MatchExprCase mc) {
    return new MatchExprCase(CloneMatcher(mc.Key), CloneExpression(mc.Value));
  }

  private MatchStmtCase CloneMatchStmtCase(MatchStmtCase mc) {
    return new MatchStmtCase(CloneMatcher(mc.Key), CloneBlockStmt(mc.Value));
  }

  private DatatypeUpdatePair CloneDatatypeUpdatePair(DatatypeUpdatePair dup) {
    return new DatatypeUpdatePair(
      (DatatypeDestructorDecl)CloneDeclRef(dup.Key), CloneExpression(dup.Value));
  }

  private Matcher CloneMatcher(Matcher m) {
    if (m is ExpressionMatcher em) {
      return new ExpressionMatcher(CloneExpression(em.E));
    } else if (m is BindingMatcher bm) {
      return new BindingMatcher(CloneVariable(bm.Var));
    } else if (m is DestructuringMatcher dsm) {
      return new DestructuringMatcher(
        constructor: (DatatypeConstructorDecl)CloneDeclRef(dsm.Constructor),
        argumentMatchers: dsm.ArgumentMatchers.Select(CloneMatcher));
    } else if (m is DisjunctiveMatcher djm) {
      return new DisjunctiveMatcher(djm.Matchers.Select(CloneMatcher));
    } else {
      throw new UnsupportedNodeCloningException(m);
    }
  }

}
