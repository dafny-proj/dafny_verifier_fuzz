namespace AST;

public abstract partial class BasicType : Type { }
public partial class BoolType : BasicType { }
public partial class CharType : BasicType { }
public partial class IntType : BasicType { }
public partial class RealType : BasicType { }

public partial class BoolType : BasicType {
  public override string BaseName => "bool";

  public readonly static BoolType Instance = new BoolType();

  private BoolType() { }
}

public partial class CharType : BasicType {
  public override string BaseName => "char";

  public readonly static CharType Instance = new CharType();

  private CharType() { }
}

public partial class IntType : BasicType {
  public override string BaseName => "int";

  public readonly static IntType Instance = new IntType();

  private IntType() { }
}

public partial class RealType : BasicType {
  public override string BaseName => "real";

  public readonly static RealType Instance = new RealType();

  private RealType() { }
}