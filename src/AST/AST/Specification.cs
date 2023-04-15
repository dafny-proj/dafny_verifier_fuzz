namespace AST;

public class Specification<S, T>
: Node, ConstructableFromDafny<Dafny.Specification<S>, Specification<S, T>>
where S : Dafny.Node where T : Node, ConstructableFromDafny<S, T> {
  public List<T> Expressions = new List<T>();

  private Specification(Dafny.Specification<S> specDafny) {
    Expressions.AddRange(specDafny.Expressions.Select(e => (T)Node.FromDafny(e)));
  }
  private Specification(List<S> specDafny) {
    Expressions.AddRange(specDafny.Select(e => (T)Node.FromDafny(e)));
  }

  public static Specification<S, T> FromDafny(List<S> dafnyNode) {
    return new Specification<S, T>(dafnyNode);
  }

  public static Specification<S, T> FromDafny(Dafny.Specification<S> dafnyNode) {
    return new Specification<S, T>(dafnyNode);
  }
}