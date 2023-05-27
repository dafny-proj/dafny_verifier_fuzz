namespace AST;

public abstract partial class Type : Node {
  public abstract string BaseName { get; }
  protected readonly List<Type> TypeArgs = new();

  public bool HasTypeArgs() => TypeArgs.Count > 0;
  public IReadOnlyList<Type> GetTypeArgs() => TypeArgs.AsReadOnly();

  public static readonly BoolType Bool = BoolType.Instance;
  public static readonly CharType Char = CharType.Instance;
  public static readonly IntType Int = IntType.Instance;
  public static readonly RealType Real = RealType.Instance;
  public static readonly NatType Nat = NatType.Instance;
  public static readonly StringType String = StringType.Instance;

  public override IEnumerable<Node> Children => TypeArgs;
}

public partial class TypeProxy : Type {
  public override string BaseName
    => throw new InvalidASTOperationException(
      $"Type proxies should not be printed.");
}
