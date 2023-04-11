namespace AST;

public class Formal : Node, ConstructableFromDafny<Dafny.Formal, Formal> {
  public string Name { get; set; }
  public Type Type { get; set; }
  public Formal(Dafny.Formal formalDafny) {
    Name = formalDafny.Name;
    Type = Type.FromDafny(formalDafny.Type);
  }
  public static Formal FromDafny(Dafny.Formal dafnyNode) {
    return new Formal(dafnyNode);
  }
}