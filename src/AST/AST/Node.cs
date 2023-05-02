namespace AST;

public interface ConstructableFromDafny<S, T> {
  static abstract T FromDafny(S dafnyNode);
  static System.Type DafnyType => typeof(S);
}

public abstract class Node : ConstructableFromDafny<Dafny.Node, Node> {
  // TODO: convert to abstract after implemented for current classes
  public virtual IEnumerable<Node> Children { 
    get => Enumerable.Empty<Node>(); 
  }
  public virtual void ReplaceChild(Node oldChild, Node newChild) {
    throw new NotSupportedException();
  }

  public static Node FromDafny(Dafny.Node dafnyNode) {
    return dafnyNode switch {
      Dafny.Program program => Program.FromDafny(program),
      Dafny.Declaration decl => Declaration.FromDafny(decl),
      Dafny.ModuleDefinition moduleDef => ModuleDefinition.FromDafny(moduleDef),
      Dafny.Statement stmt => Statement.FromDafny(stmt),
      Dafny.Expression expr => Expression.FromDafny(expr),
      Dafny.Formal formal => Formal.FromDafny(formal),
      Dafny.BoundVar boundVar => BoundVar.FromDafny(boundVar),
      Dafny.Type type => Type.FromDafny(type),
      Dafny.AssignmentRhs assignRhs => AssignmentRhs.FromDafny(assignRhs),
      // FIXME: Dafny.Specification<?> => ?
      _ => throw new NotImplementedException($"Unhandled translation from Dafny for `{dafnyNode.GetType()}`"),
    };
  }
}