namespace AST_new;

public partial class BinaryExpr : Expression {
  public Expression E0 { get; }
  public Expression E1 { get; }
  public BinaryExpr.Opcode Op { get; }
  public override Type Type
    => OpcodesReturningBool.Contains(Op) ? Type.Bool : E0.Type;

  public BinaryExpr(BinaryExpr.Opcode op, Expression e0, Expression e1) {
    E0 = e0;
    E1 = e1;
    Op = op;
  }

  public override IEnumerable<Node> Children => new[] { E0, E1 };
}

public partial class BinaryExpr : Expression {
  public enum Opcode {
    Iff, Imp, Exp, And, Or, Eq, Neq, Lt, Le, Ge, Gt, Disjoint, In, NotIn, Add,
    Sub, Mul, Div, Mod, LeftShift, RightShift, BitwiseAnd, BitwiseOr, BitwiseXor
  }

  public HashSet<Opcode> OpcodesReturningBool = new() {
     Opcode.Eq, Opcode.Neq, Opcode.Lt, Opcode.Le, Opcode.Gt, Opcode.Ge,
     Opcode.Disjoint, Opcode.In, Opcode.NotIn
  };

  public static string OpcodeAsString(Opcode op) {
    return op switch {
      Opcode.Iff => "<==>",
      Opcode.Imp => "==>",
      Opcode.Exp => "<==",
      Opcode.And => "&&",
      Opcode.Or => "||",
      Opcode.Eq => "==",
      Opcode.Neq => "!=",
      Opcode.Lt => "<",
      Opcode.Le => "<=",
      Opcode.Ge => ">=",
      Opcode.Gt => ">",
      Opcode.Disjoint => "!!",
      Opcode.In => "in",
      Opcode.NotIn => "!in",
      Opcode.LeftShift => "<<",
      Opcode.RightShift => ">>",
      Opcode.Add => "+",
      Opcode.Sub => "-",
      Opcode.Mul => "*",
      Opcode.Div => "/",
      Opcode.Mod => "%",
      Opcode.BitwiseAnd => "&",
      Opcode.BitwiseOr => "|",
      Opcode.BitwiseXor => "^",
      _ => throw new UnsupportedASTOperationException(
        op, "opcode to string conversion"),
    };
  }
}
