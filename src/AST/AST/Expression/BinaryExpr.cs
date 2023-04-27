namespace AST;

public class BinaryExpr
: Expression, ConstructableFromDafny<Dafny.BinaryExpr, BinaryExpr> {
  public override IEnumerable<Node> Children => new Node[] { E0, E1 };
  public enum Opcode {
    Iff,
    Imp,
    Exp,
    And,
    Or,
    Eq,
    Neq,
    Lt,
    Le,
    Ge,
    Gt,
    Disjoint,
    In,
    NotIn,
    LeftShift,
    RightShift,
    Add,
    Sub,
    Mul,
    Div,
    Mod,
    BitwiseAnd,
    BitwiseOr,
    BitwiseXor
  };
  public static Opcode FromDafny(Dafny.BinaryExpr.Opcode opDafny) {
    return opDafny switch {
      Dafny.BinaryExpr.Opcode.Iff => Opcode.Iff,
      Dafny.BinaryExpr.Opcode.Imp => Opcode.Imp,
      Dafny.BinaryExpr.Opcode.Exp => Opcode.Exp,
      Dafny.BinaryExpr.Opcode.And => Opcode.And,
      Dafny.BinaryExpr.Opcode.Or => Opcode.Or,
      Dafny.BinaryExpr.Opcode.Eq => Opcode.Eq,
      Dafny.BinaryExpr.Opcode.Neq => Opcode.Neq,
      Dafny.BinaryExpr.Opcode.Lt => Opcode.Lt,
      Dafny.BinaryExpr.Opcode.Le => Opcode.Le,
      Dafny.BinaryExpr.Opcode.Ge => Opcode.Ge,
      Dafny.BinaryExpr.Opcode.Gt => Opcode.Gt,
      Dafny.BinaryExpr.Opcode.Disjoint => Opcode.Disjoint,
      Dafny.BinaryExpr.Opcode.In => Opcode.In,
      Dafny.BinaryExpr.Opcode.NotIn => Opcode.NotIn,
      Dafny.BinaryExpr.Opcode.LeftShift => Opcode.LeftShift,
      Dafny.BinaryExpr.Opcode.RightShift => Opcode.RightShift,
      Dafny.BinaryExpr.Opcode.Add => Opcode.Add,
      Dafny.BinaryExpr.Opcode.Sub => Opcode.Sub,
      Dafny.BinaryExpr.Opcode.Mul => Opcode.Mul,
      Dafny.BinaryExpr.Opcode.Div => Opcode.Div,
      Dafny.BinaryExpr.Opcode.Mod => Opcode.Mod,
      Dafny.BinaryExpr.Opcode.BitwiseAnd => Opcode.BitwiseAnd,
      Dafny.BinaryExpr.Opcode.BitwiseOr => Opcode.BitwiseOr,
      Dafny.BinaryExpr.Opcode.BitwiseXor => Opcode.BitwiseXor,
      _ => throw new InvalidOperationException(),
    };
  }

  public static string OpcodeString(Opcode op) {
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
      _ => throw new InvalidOperationException(),
    };
  }

  public Opcode Op { get; set; }
  public Expression E0 { get; set; }
  public Expression E1 { get; set; }
  public Type _Type;
  public override Type Type { get => _Type; }

  private Type GetBinExprType() {
    switch (Op) {
      case Opcode.Eq:
      case Opcode.Neq:
      case Opcode.Lt:
      case Opcode.Le:
      case Opcode.Gt:
      case Opcode.Ge:
      case Opcode.Disjoint:
      case Opcode.In:
      case Opcode.NotIn:
        return Type.Bool;
      default:
        return E0.Type;
    }
  }

  public BinaryExpr(Opcode op, Expression e0, Expression e1) {
    Op = op;
    E0 = e0;
    E1 = e1;
    _Type = GetBinExprType();
  }

  private BinaryExpr(Dafny.BinaryExpr binaryExprDafny)
  : this(/*op=*/FromDafny(binaryExprDafny.Op),
         /*e0=*/Expression.FromDafny(binaryExprDafny.E0),
         /*e1=*/Expression.FromDafny(binaryExprDafny.E1)) { }

  public static BinaryExpr FromDafny(Dafny.BinaryExpr dafnyNode) {
    return new BinaryExpr(dafnyNode);
  }

  public static List<BinaryExpr.Opcode> GetEquivOperands(Opcode op) {
    return op switch {
      Opcode.Add => new List<Opcode>() { Opcode.Sub },
      Opcode.Sub => new List<Opcode>() { Opcode.Add },
      _ => throw new NotImplementedException(),
    };
  }
  public static bool HasTypeEquivOperands(Opcode op) {
    return GetEquivOperands(op).Count > 0;
  }
}
