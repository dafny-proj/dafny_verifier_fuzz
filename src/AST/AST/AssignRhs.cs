using System.Diagnostics.Contracts;

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

// See Dafny documentation for constructs represented by TypeRhs
public abstract class TypeRhs
: AssignmentRhs, ConstructableFromDafny<Dafny.TypeRhs, TypeRhs> {
  public static TypeRhs FromDafny(Dafny.TypeRhs dafnyNode) {
    if (NewArrayRHS.IsNewArrayRHS(dafnyNode)) {
      return NewArrayRHS.FromDafny(dafnyNode);
    }
    throw new NotImplementedException();
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