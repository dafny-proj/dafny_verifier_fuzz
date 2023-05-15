using System.Diagnostics.Contracts;

namespace AST;

// See Dafny documentation for constructs represented by TypeRhs
public abstract class TypeRhs
: AssignmentRhs, ConstructableFromDafny<Dafny.TypeRhs, TypeRhs> {
  public static TypeRhs FromDafny(Dafny.TypeRhs dafnyNode) {
    if (NewArrayRHS.IsNewArrayRHS(dafnyNode)) {
      return NewArrayRHS.FromDafny(dafnyNode);
    }
    if (ConstructorCallRhs.IsConstructorCallRHS(dafnyNode)) {
      return ConstructorCallRhs.FromDafny(dafnyNode);
    }
    throw new NotImplementedException();
  }
}

public class ConstructorCallRhs
: TypeRhs, ConstructableFromDafny<Dafny.TypeRhs, ConstructorCallRhs> {
  public CallStmt ConstructorCall { get; }

  public Constructor GetConstructor() {
    return (ConstructorCall.Callee.Member as Constructor)!;
  }

  public ConstructorCallRhs(CallStmt constructorCall) {
    Contract.Requires(constructorCall.Callee.Member is Constructor);
    ConstructorCall = constructorCall;
  }

  private ConstructorCallRhs(Dafny.TypeRhs trd)
  : this(CallStmt.FromDafny(trd.InitCall)) { }

  public static new ConstructorCallRhs FromDafny(Dafny.TypeRhs dafnyNode) {
    return new ConstructorCallRhs(dafnyNode);
  }

  public static bool IsConstructorCallRHS(Dafny.TypeRhs trd) {
    return trd.InitCall != null;
  }
}

/// If ArrayDimensions != null, then the TypeRhs represents "new EType[ArrayDimensions]",
///     ElementInit is non-null to represent "new EType[ArrayDimensions] (elementInit)",
///     InitDisplay is non-null to represent "new EType[ArrayDimensions] [InitDisplay]",
/// NewArrayRHS
///  * new T[EE]
///    This allocates an array of objects of type T (where EE is a list of expression)
///  * new T[EE] (elementInit)
///    This is like the previous, but uses "elementInit" to initialize the elements of the new array.
///  * new T[E] [EE]
///    This is like the first one, but uses the elements displayed in the list EE as the initial
///    elements of the array.  Only a 1-dimensional array may be used in this case.  The size denoted
///    by E must equal the length of EE.
public class NewArrayRHS
: TypeRhs, ConstructableFromDafny<Dafny.TypeRhs, NewArrayRHS> {
  public override IEnumerable<Node> Children {
    get {
      var children = new[] { ElementType };
      children.Concat<Node>(ArrayDimensions);
      if (ElementInit != null) {
        children.Append<Node>(ElementInit);
      }
      if (ListInit != null) {
        children.Concat<Node>(ListInit);
      }
      return children;
    }
  }

  public Type ElementType { get; set; }
  public List<Expression> ArrayDimensions = new List<Expression>();
  public Expression? ElementInit;
  public List<Expression>? ListInit;

  private NewArrayRHS(Dafny.TypeRhs trd) {
    ElementType = Type.FromDafny(trd.EType);
    ArrayDimensions.AddRange(trd.ArrayDimensions.Select(Expression.FromDafny));
    ElementInit = trd.ElementInit == null ? null
      : Expression.FromDafny(trd.ElementInit);
    ListInit = trd.InitDisplay == null ? null
      : trd.InitDisplay.Select(Expression.FromDafny).ToList();
  }

  public static new NewArrayRHS FromDafny(Dafny.TypeRhs dafnyNode) {
    Contract.Assert(IsNewArrayRHS(dafnyNode));
    return new NewArrayRHS(dafnyNode);
  }

  public static bool IsNewArrayRHS(Dafny.TypeRhs trd) {
    return trd.ArrayDimensions != null;
  }
}