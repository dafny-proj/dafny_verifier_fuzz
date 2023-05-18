using System.Numerics;

namespace AST_new;

public partial class BoolLiteralExpr : LiteralExpr { }
public partial class IntLiteralExpr : LiteralExpr { }
public partial class StringLiteralExpr : LiteralExpr { }

public partial class BoolLiteralExpr : LiteralExpr {
  public bool Value { get; }
  public override Type Type { get => Type.Bool; }

  public BoolLiteralExpr(bool value) {
    Value = value;
  }
}

public partial class IntLiteralExpr : LiteralExpr {
  public BigInteger Value { get; }
  public override Type Type { get => Type.Int; }

  public IntLiteralExpr(BigInteger value) {
    Value = value;
  }
}

public partial class StringLiteralExpr : LiteralExpr {
  public string Value { get; }
  public override Type Type { get => Type.String; }

  public StringLiteralExpr(string value) {
    Value = value;
  }
}
