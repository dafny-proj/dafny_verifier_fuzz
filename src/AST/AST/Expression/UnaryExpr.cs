namespace AST;

public class UnaryExpr
: Expression, ConstructableFromDafny<Dafny.UnaryOpExpr, UnaryExpr> {
  // TODO: implement remaining unary operators.
  public enum Opcode {
    Not,
  }

  public static Opcode FromDafny(Dafny.UnaryOpExpr.Opcode opDafny) {
    return opDafny switch {
      Dafny.UnaryOpExpr.Opcode.Not => Opcode.Not,
      _ => throw new NotImplementedException()
    };
  }

  public static string OpcodeString(Opcode op) {
    return op switch {
      Opcode.Not => "!",
      _ => throw new NotImplementedException()
    };
  }

  private Type GetUnExprType() {
    return Op switch {
      Opcode.Not => E.Type,
      _ => throw new NotImplementedException(),
    };
  }

  public Opcode Op { get; set; }
  public Expression E { get; set; }
  private Type _Type;
  public override Type Type => _Type;

  public UnaryExpr(Opcode op, Expression e) {
    Op = op;
    E = e;
    _Type = GetUnExprType();
  }

  private UnaryExpr(Dafny.UnaryOpExpr ued)
  : this(FromDafny(ued.Op), Expression.FromDafny(ued.E)) { }

  public static UnaryExpr FromDafny(Dafny.UnaryOpExpr dafnyNode) {
    return new UnaryExpr(dafnyNode);
  }

  public static UnaryExpr CreateNegExpr(Expression e) {
    return new UnaryExpr(Opcode.Not, e);
  }
}