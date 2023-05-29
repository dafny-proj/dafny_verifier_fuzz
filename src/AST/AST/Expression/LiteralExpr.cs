using System.Numerics;

namespace AST;

public abstract partial class LiteralExpr : Expression { }
public partial class BoolLiteralExpr : LiteralExpr { }
public partial class CharLiteralExpr : LiteralExpr { }
public partial class IntLiteralExpr : LiteralExpr { }
public partial class RealLiteralExpr : LiteralExpr { }
public partial class StringLiteralExpr : LiteralExpr { }
public partial class NullLiteralExpr : LiteralExpr { }

public abstract partial class LiteralExpr : Expression {
  public override IEnumerable<Node> Children => Enumerable.Empty<Node>();
}

public partial class BoolLiteralExpr : LiteralExpr {
  public bool Value { get; }
  public override Type Type => Type.Bool;

  public BoolLiteralExpr(bool value) {
    Value = value;
  }
}

public partial class CharLiteralExpr : LiteralExpr {
  public string Value { get; }
  public override Type Type => Type.Char;

  public CharLiteralExpr(string value) {
    Value = value;
  }
}

public partial class IntLiteralExpr : LiteralExpr {
  public BigInteger Value { get; }
  public override Type Type => Type.Int;

  public IntLiteralExpr(BigInteger value) {
    Value = value;
  }
}

public partial class RealLiteralExpr : LiteralExpr {
  // Official Dafny uses Microsoft.BaseTypes.BigDec (from Boogie).
  // For now, there is no advanced used of reals, storing as string suffices.
  public string Value { get; }
  public override Type Type => Type.Real;

  public RealLiteralExpr(string value) {
    Value = value;
  }
}

public partial class StringLiteralExpr : LiteralExpr {
  public string Value { get; }
  public override Type Type => Type.String;

  public StringLiteralExpr(string value) {
    Value = value;
  }
}

public partial class NullLiteralExpr : LiteralExpr {
  private Type _type { get; }
  public override Type Type => _type;

  public NullLiteralExpr(Type type) {
    _type = type;
  }
}
