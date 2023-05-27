using System.Diagnostics.Contracts;

namespace AST;

public abstract partial class AssignmentRhs : Node { }
public partial class ExprRhs : AssignmentRhs { }
public partial class MethodCallRhs : AssignmentRhs { }
// new T[dim_0, dim_1, ...]
public partial class NewArrayRhs : AssignmentRhs { }
// new T[dim_0, dim_1, ...](expr_init)
public partial class NewArrayWithElementInitialiserRhs : NewArrayRhs { }
// new T[n][expr_0, expr_1, ..., expr_n]
public partial class NewArrayWithListInitialiserRhs : NewArrayRhs { }
// new C
public partial class NewObjectRhs : AssignmentRhs { }
// new C.Constructor(arg_0, arg_1, ...)
public partial class NewObjectWithConstructorRhs : NewObjectRhs { }

public partial class ExprRhs : AssignmentRhs {
  public Expression E { get; set; }

  public ExprRhs(Expression e) {
    E = e;
  }

  public override IEnumerable<Node> Children => new[] { E };
}

public partial class MethodCallRhs : AssignmentRhs {
  public MemberSelectExpr Callee { get; set; }
  public readonly List<Expression> Arguments = new();

  public MethodCallRhs(MemberSelectExpr callee,
  IEnumerable<Expression>? arguments = null) {
    Contract.Requires(callee.Member is MethodDecl);
    Callee = callee;
    if (arguments != null) {
      Arguments.AddRange(arguments);
    }
  }

  public override IEnumerable<Node> Children
    => new[] { Callee }.Concat<Node>(Arguments);
}

public partial class NewArrayRhs : AssignmentRhs {
  public Type ElementType { get; }
  public readonly List<Expression> Dimensions = new();

  public NewArrayRhs(Type elementType, IEnumerable<Expression> dimensions) {
    ElementType = elementType;
    Dimensions.AddRange(dimensions);
  }

  public override IEnumerable<Node> Children
    => new[] { ElementType }.Concat<Node>(Dimensions);
}

public partial class NewArrayWithElementInitialiserRhs : NewArrayRhs {
  // An expression that returns a function of type `index: nat -> element: T`.
  public Expression ElementInitialiser { get; set; }

  public NewArrayWithElementInitialiserRhs(Type elementType,
  IEnumerable<Expression> dimensions, Expression elementInitialiser)
  : base(elementType, dimensions) {
    ElementInitialiser = elementInitialiser;
  }

  public override IEnumerable<Node> Children
    => base.Children.Append(ElementInitialiser);
}

// Only for 1 dimensional arrays.
// Size of list initialiser must match stated dimension.
public partial class NewArrayWithListInitialiserRhs : NewArrayRhs {
  public readonly List<Expression> ListInitialiser = new();

  public NewArrayWithListInitialiserRhs(Type elementType,
  Expression dimension, IEnumerable<Expression> listInitialiser)
  : base(elementType, new[] { dimension }) {
    ListInitialiser.AddRange(listInitialiser);
  }

  public override IEnumerable<Node> Children
    => base.Children.Concat(ListInitialiser);
}

public partial class NewObjectRhs : AssignmentRhs {
  public Type ObjectType { get; }

  public NewObjectRhs(Type objectType) {
    ObjectType = objectType;
  }

  public override IEnumerable<Node> Children => new[] { ObjectType };
}

public partial class NewObjectWithConstructorRhs : NewObjectRhs {
  public ConstructorDecl Constructor { get; }
  public readonly List<Expression> ConstructorArguments = new();

  public NewObjectWithConstructorRhs(Type objectType,
  ConstructorDecl constructor,
  IEnumerable<Expression>? constructorArguments = null) : base(objectType) {
    Constructor = constructor;
    if (constructorArguments != null) {
      ConstructorArguments.AddRange(constructorArguments);
    }
  }

  public override IEnumerable<Node> Children
    => base.Children.Concat<Node>(ConstructorArguments);
}
