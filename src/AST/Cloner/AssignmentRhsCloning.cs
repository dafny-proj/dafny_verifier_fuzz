namespace AST.Cloner;

public partial class ASTCloner {
  private AssignmentRhs CloneAssignmentRhs(AssignmentRhs a) {
    return a switch {
      ExprRhs ea => CloneExprRhs(ea),
      MethodCallRhs ma => CloneMethodCallRhs(ma),
      NewArrayRhs na => CloneNewArrayRhs(na),
      NewObjectRhs na => CloneNewObjectRhs(na),
      _ => throw new UnsupportedNodeCloningException(a),
    };
  }

  private ExprRhs CloneExprRhs(ExprRhs a) {
    return new ExprRhs(CloneExpression(a.E));
  }

  private MethodCallRhs CloneMethodCallRhs(MethodCallRhs a) {
    return new MethodCallRhs(
      CloneMemberSelectExpr(a.Callee), a.Arguments.Select(CloneExpression));
  }

  private NewArrayRhs CloneNewArrayRhs(NewArrayRhs a) {
    var et = CloneType(a.ElementType);
    var dim = a.Dimensions.Select(CloneExpression);
    return a switch {
      NewArrayWithElementInitialiserRhs ae
        => new NewArrayWithElementInitialiserRhs(
          elementType: et, dimensions: dim,
          elementInitialiser: CloneExpression(ae.ElementInitialiser)),
      NewArrayWithListInitialiserRhs al
        => new NewArrayWithListInitialiserRhs(
          elementType: et, dimension: dim.ElementAt(0),
          listInitialiser: al.ListInitialiser.Select(CloneExpression)),
      _ => new NewArrayRhs(elementType: et, dimensions: dim),
    };
  }

  private NewObjectRhs CloneNewObjectRhs(NewObjectRhs a) {
    var ot = CloneType(a.ObjectType);
    return a switch {
      NewObjectWithConstructorRhs ac
        => new NewObjectWithConstructorRhs(objectType: ot,
          constructor: (ConstructorDecl)CloneDeclRef(ac.Constructor),
          constructorArguments: ac.ConstructorArguments.Select(CloneExpression)),
      _ => new NewObjectRhs(objectType: ot),
    };
  }

}
