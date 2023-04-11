namespace AST;

public abstract class Expression
: Node, ConstructableFromDafny<Dafny.Expression, Expression> {
  public static Expression FromDafny(Dafny.Expression dafnyNode) {
    return dafnyNode switch {
      Dafny.NameSegment nameSeg
        => NameSegment.FromDafny(nameSeg),
      Dafny.BinaryExpr binExpr
        => BinaryExpr.FromDafny(binExpr),
      _ => throw new NotImplementedException(),
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
    Add,
  };
  public static Opcode FromDafny(Dafny.BinaryExpr.Opcode opDafny) {
    return opDafny switch {
      Dafny.BinaryExpr.Opcode.Add => Opcode.Add,
      _ => throw new NotImplementedException(),
    };
  }

  public static string OpcodeString(Opcode op) {
    return op switch {
      Opcode.Add => "+",
      _ => throw new NotImplementedException(),
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
}