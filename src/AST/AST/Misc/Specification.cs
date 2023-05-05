namespace AST;

public class Specification<S, T>
: Node, ConstructableFromDafny<Dafny.Specification<S>, Specification<S, T>>
where S : Dafny.Node where T : Node, ConstructableFromDafny<S, T> {
  public override IEnumerable<Node> Children => Expressions;

  public List<T> Expressions = new List<T>();

  private Specification(Dafny.Specification<S> specDafny) {
    if (specDafny.Expressions != null) {
      Expressions.AddRange(specDafny.Expressions.Select(e => (T)Node.FromDafny(e)));
    }
  }
  private Specification(IEnumerable<S> specDafny) {
    Expressions.AddRange(specDafny.Select(e => (T)Node.FromDafny(e)));
  }
  private Specification(IEnumerable<T> spec) {
    Expressions.AddRange(spec);
  }

  public static Specification<S, T> FromDafny(IEnumerable<S> dafnyNode) {
    return new Specification<S, T>(dafnyNode);
  }

  public static Specification<S, T> FromDafny(Dafny.Specification<S> dafnyNode) {
    return new Specification<S, T>(dafnyNode);
  }

  public Specification<S, T> GetProvided() {
    return new Specification<S, T>(Expressions.Where(e => e is not AutoGeneratedExpression));
  }

  public Specification<S, T> GetInferred() {
    return new Specification<S, T>(Expressions.Where(e => e is AutoGeneratedExpression));
  }

  public override Specification<S, T> Clone() {
    return new Specification<S, T>(Expressions.Select(e => (T)e.Clone()));
  }
}