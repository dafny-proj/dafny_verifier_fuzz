namespace AST;

public partial class UnaryExpr : Expression {
  public Expression E { get; set; }
  public UnaryExpr.Opcode Op { get; }
  public override Type Type {
    get {
      switch (Op) {
        case Opcode.Not:
          return E.Type;
        case Opcode.Cardinality:
          return Type.Int;
        case Opcode.Fresh:
        case Opcode.Allocated:
          return Type.Bool;
        default:
          throw new UnsupportedASTOperationException(
            this, "expression type retrieval");
      }
    }
  }

  public UnaryExpr(UnaryExpr.Opcode op, Expression e) {
    Op = op;
    E = e;
  }

  public override IEnumerable<Node> Children => new[] { E };
}

public partial class UnaryExpr : Expression {
  public enum Opcode {
    Not, Cardinality, Fresh, Allocated
  }
}