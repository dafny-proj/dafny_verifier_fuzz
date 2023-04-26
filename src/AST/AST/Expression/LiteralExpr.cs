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
