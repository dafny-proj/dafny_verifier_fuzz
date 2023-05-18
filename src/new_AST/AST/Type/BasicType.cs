namespace AST_new;

public partial class BoolType : BasicType {
  public override string BaseName => "bool";

  public static BoolType Instance => new BoolType();

  private BoolType() { }
}

public partial class CharType : BasicType {
  public override string BaseName => "char";

  public static CharType Instance => new CharType();

  private CharType() { }
}

public partial class IntType : BasicType {
  public override string BaseName => "int";

  public static IntType Instance => new IntType();

  private IntType() { }
}

public partial class RealType : BasicType {
  public override string BaseName => "real";

  public static RealType Instance => new RealType();

  private RealType() { }
}