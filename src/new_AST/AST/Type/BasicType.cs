namespace AST_new;

public partial class BoolType : BasicType {
  public override string BaseName => "bool";

  public static BoolType Instance => new BoolType();

  private BoolType() { }
}

public partial class IntType : BasicType {
  public override string BaseName => "int";

  public static IntType Instance => new IntType();

  private IntType() { }
}
