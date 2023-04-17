using System.Numerics;
using System.Diagnostics.Contracts;

namespace AST;

public abstract class Expression
: Node, ConstructableFromDafny<Dafny.Expression, Expression> {
  public static Expression FromDafny(Dafny.Expression dafnyNode) {
    return dafnyNode switch {
      Dafny.NameSegment nameSeg
        => NameSegment.FromDafny(nameSeg),
      Dafny.BinaryExpr binExpr
        => BinaryExpr.FromDafny(binExpr),
      Dafny.LiteralExpr litExpr
        => LiteralExpr.FromDafny(litExpr),
      Dafny.ParensExpression parensExpr
        => ParensExpression.FromDafny(parensExpr),
      Dafny.NegationExpression negExpr
        => NegationExpression.FromDafny(negExpr),
      Dafny.IdentifierExpr identExpr
        => IdentifierExpr.FromDafny(identExpr),
      Dafny.ApplySuffix applySuffix
        => ApplySuffix.FromDafny(applySuffix),
      _ => throw new NotImplementedException($"{dafnyNode.GetType()}"),
    };
  }
}

public class NameSegment
: Expression, ConstructableFromDafny<Dafny.NameSegment, NameSegment> {
  public string Name { get; set; }
  private NameSegment(Dafny.NameSegment nameSegmentDafny) {
    Name = nameSegmentDafny.Name;
  }
  public static NameSegment FromDafny(Dafny.NameSegment dafnyNode) {
    return new NameSegment(dafnyNode);
  }
}

public class BinaryExpr
: Expression, ConstructableFromDafny<Dafny.BinaryExpr, BinaryExpr> {
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
  private BinaryExpr(Dafny.BinaryExpr binaryExprDafny) {
    Op = FromDafny(binaryExprDafny.Op);
    E0 = Expression.FromDafny(binaryExprDafny.E0);
    E1 = Expression.FromDafny(binaryExprDafny.E1);
  }
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

public abstract class LiteralExpr
: Expression, ConstructableFromDafny<Dafny.LiteralExpr, LiteralExpr> {
  public static LiteralExpr FromDafny(Dafny.LiteralExpr dafnyNode) {
    if (dafnyNode is Dafny.StaticReceiverExpr sred) {
      return StaticReceiverExpr.FromDafny(sred);
    }
    if (dafnyNode.Value is BigInteger) {
      return IntLiteralExpr.FromDafny(dafnyNode);
    }
    if (dafnyNode.Value is bool) {
      return BoolLiteralExpr.FromDafny(dafnyNode);
    }
    throw new NotImplementedException();
  }
}

public class IntLiteralExpr
: LiteralExpr, ConstructableFromDafny<Dafny.LiteralExpr, IntLiteralExpr> {
  public BigInteger Value { get; set; }
  private IntLiteralExpr(Dafny.LiteralExpr literalExprDafny) {
    Value = (BigInteger)literalExprDafny.Value;
  }
  public static new IntLiteralExpr FromDafny(Dafny.LiteralExpr dafnyNode) {
    return new IntLiteralExpr(dafnyNode);
  }
}

public class BoolLiteralExpr
: LiteralExpr, ConstructableFromDafny<Dafny.LiteralExpr, BoolLiteralExpr> {
  public bool Value { get; set; }
  private BoolLiteralExpr(Dafny.LiteralExpr literalExprDafny) {
    Value = (bool)literalExprDafny.Value;
  }
  public static new BoolLiteralExpr FromDafny(Dafny.LiteralExpr dafnyNode) {
    return new BoolLiteralExpr(dafnyNode);
  }
}

public class AttributedExpression
: Node, ConstructableFromDafny<Dafny.AttributedExpression, AttributedExpression> {
  // TODO: Attributes, Label
  public Expression E;
  private AttributedExpression(Dafny.AttributedExpression attributedExprDafny) {
    E = Expression.FromDafny(attributedExprDafny.E);
  }
  public static AttributedExpression FromDafny(Dafny.AttributedExpression dafnyNode) {
    return new AttributedExpression(dafnyNode);
  }
}

public class FrameExpression
: Node, ConstructableFromDafny<Dafny.FrameExpression, FrameExpression> {
  // TODO: FieldName
  public Expression E; // pre-resolution
  private FrameExpression(Dafny.FrameExpression frameExprDafny) {
    E = Expression.FromDafny(frameExprDafny.E);
  }
  public static FrameExpression FromDafny(Dafny.FrameExpression dafnyNode) {
    return new FrameExpression(dafnyNode);
  }
}

public class ParensExpression
: Expression, ConstructableFromDafny<Dafny.ParensExpression, ParensExpression> {
  public Expression E { get; set; }

  protected ParensExpression(Dafny.ParensExpression parensExprDafny) {
    E = Expression.FromDafny(parensExprDafny.E);
  }

  public static ParensExpression FromDafny(Dafny.ParensExpression dafnyNode) {
    if (dafnyNode is Dafny.AutoGeneratedExpression ageDafny) {
      return AutoGeneratedExpression.FromDafny(ageDafny);
    }
    return new ParensExpression(dafnyNode);
  }
}

// Expressions that are not derived from the program text.
// AutoGeneratedExpressions may originate from e.g. clause inference.
public class AutoGeneratedExpression
: ParensExpression, ConstructableFromDafny<Dafny.AutoGeneratedExpression, AutoGeneratedExpression> {
  private AutoGeneratedExpression(Dafny.AutoGeneratedExpression autoGenExprDafny)
  : base(autoGenExprDafny) { }

  public static AutoGeneratedExpression FromDafny(Dafny.AutoGeneratedExpression dafnyNode) {
    return new AutoGeneratedExpression(dafnyNode);
  }
}

public class NegationExpression
: Expression, ConstructableFromDafny<Dafny.NegationExpression, NegationExpression> {
  public Expression E { get; set; }

  private NegationExpression(Dafny.NegationExpression negExprDafny) {
    E = Expression.FromDafny(negExprDafny.E);
  }

  public static NegationExpression FromDafny(Dafny.NegationExpression dafnyNode) {
    return new NegationExpression(dafnyNode);
  }
}

// Subclasses: AutoGhostIdentExpr, ResolverIdentExpr
public class IdentifierExpr
: Expression, ConstructableFromDafny<Dafny.IdentifierExpr, IdentifierExpr> {
  public string Name { get; set; }

  private IdentifierExpr(Dafny.IdentifierExpr identExprDafny) {
    Name = identExprDafny.Name;
  }

  public static IdentifierExpr FromDafny(Dafny.IdentifierExpr dafnyNode) {
    return new IdentifierExpr(dafnyNode);
  }
}


public class MemberSelectExpr
: Expression, ConstructableFromDafny<Dafny.MemberSelectExpr, MemberSelectExpr> {
  public Expression Receiver { get; set; }
  public bool ReceiverIsImplicit { get; }
  public string MemberName { get; set; }
  public MemberDecl Member { get; set; } // TODO: Is this field needed?

  private MemberSelectExpr(Dafny.MemberSelectExpr mse) {
    Receiver = Expression.FromDafny(mse.Obj);
    // TODO: is it better to check the type of the receiver for deducing implicitness?
    ReceiverIsImplicit = mse.Obj.IsImplicit;
    MemberName = mse.MemberName;
    Member = MemberDecl.FromDafny(mse.Member);
  }

  public static MemberSelectExpr FromDafny(Dafny.MemberSelectExpr dafnyNode) {
    return new MemberSelectExpr(dafnyNode);
  }
}

public class StaticReceiverExpr
: LiteralExpr, ConstructableFromDafny<Dafny.StaticReceiverExpr, StaticReceiverExpr> {
  public Type Type { get; set; }
  public bool IsImplicit { get; set; }

  private StaticReceiverExpr(Dafny.StaticReceiverExpr sred) {
    Type = Type.FromDafny(sred.Type);
    IsImplicit = sred.IsImplicit;
  }

  public static StaticReceiverExpr FromDafny(Dafny.StaticReceiverExpr dafnyNode) {
    Contract.Requires(dafnyNode.WasResolved()); // for type to be non-null.
    return new StaticReceiverExpr(dafnyNode);
  }
}

public class ApplySuffix
: Expression, ConstructableFromDafny<Dafny.ApplySuffix, ApplySuffix> {
  public Expression Lhs { get; set; }
  public ArgumentBindings ArgumentBindings { get; set; }

  private ApplySuffix(Dafny.ApplySuffix asd) {
    Lhs = Expression.FromDafny(asd.Lhs);
    ArgumentBindings = ArgumentBindings.FromDafny(asd.Bindings);
  }

  public static ApplySuffix FromDafny(Dafny.ApplySuffix dafnyNode) {
    return new ApplySuffix(dafnyNode);
  }
}
