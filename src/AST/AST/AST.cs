global using Dafny = Microsoft.Dafny;

namespace AST;

public interface ConstructableFromDafny<S, T> {
  static abstract T FromDafny(S dafnyNode);
  static System.Type DafnyType => typeof(S);
}

public abstract class Node : ConstructableFromDafny<Dafny.Node, Node> {
  public static Node FromDafny(Dafny.Node dafnyNode) {
    return dafnyNode switch {
      Dafny.Program program => Program.FromDafny(program),
      Dafny.Declaration decl => Declaration.FromDafny(decl),
      Dafny.ModuleDefinition moduleDef => ModuleDefinition.FromDafny(moduleDef),
      Dafny.Statement stmt => Statement.FromDafny(stmt),
      Dafny.Expression expr => Expression.FromDafny(expr),
      Dafny.Formal formal => Formal.FromDafny(formal),
      Dafny.Type type => Type.FromDafny(type),
      Dafny.AssignmentRhs assignRhs => AssignmentRhs.FromDafny(assignRhs),
      // FIXME: Dafny.Specification<?> => ?
      _ => throw new NotImplementedException($"{dafnyNode.GetType()}"),
    };
  }
}
