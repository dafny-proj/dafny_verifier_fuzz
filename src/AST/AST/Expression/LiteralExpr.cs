using System.Numerics;

namespace AST;

public abstract partial class LiteralExpr : Expression { }
public partial class BoolLiteralExpr : LiteralExpr { }
public partial class IntLiteralExpr : LiteralExpr { }
public partial class StringLiteralExpr : LiteralExpr { }

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

public partial class IntLiteralExpr : LiteralExpr {
  public BigInteger Value { get; }
  public override Type Type => Type.Int;

  public IntLiteralExpr(BigInteger value) {
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
