namespace AST;

// { x [: Type] [<- Domain] [Attributes] [| Range] }
public partial class QuantifierDomain : Node {
  // There is syntax loss during parsing.
  // x1 <- C1 | E1, ..., xN <- CN | EN
  // Is converted to:
  // x1, ... xN | x1 in C1 && E1 && ... && xN in CN && EN
  public readonly List<BoundVar> Vars = new();
  public Expression? Range { get; set; }

  public QuantifierDomain(IEnumerable<BoundVar> vars, Expression? range) {
    Vars.AddRange(vars);
    Range = range;
  }

  public override IEnumerable<Node> Children
    => Range != null ? Vars.Append<Node>(Range) : Vars;
}
