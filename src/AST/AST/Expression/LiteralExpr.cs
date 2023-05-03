using System.Numerics;
using System.Diagnostics.Contracts;

namespace AST;

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
  public override Type Type { get => Type.Int; }

  public IntLiteralExpr(BigInteger value) {
    Value = value;
  }

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
  public override Type Type { get => Type.Bool; }

  public BoolLiteralExpr(bool value) {
    Value = value;
  }
  private BoolLiteralExpr(Dafny.LiteralExpr literalExprDafny)
  : this((bool)literalExprDafny.Value) { }

  public static new BoolLiteralExpr FromDafny(Dafny.LiteralExpr dafnyNode) {
    return new BoolLiteralExpr(dafnyNode);
  }
}

public class StaticReceiverExpr
: LiteralExpr, ConstructableFromDafny<Dafny.StaticReceiverExpr, StaticReceiverExpr> {
  private Type _Type;
  public override Type Type { get => _Type; }
  public bool IsImplicit { get; set; }

  private StaticReceiverExpr(Dafny.StaticReceiverExpr sred) {
    _Type = Type.FromDafny(sred.Type);
    IsImplicit = sred.IsImplicit;
  }

  public static StaticReceiverExpr FromDafny(Dafny.StaticReceiverExpr dafnyNode) {
    Contract.Requires(dafnyNode.WasResolved()); // for type to be non-null.
    return new StaticReceiverExpr(dafnyNode);
  }
}
