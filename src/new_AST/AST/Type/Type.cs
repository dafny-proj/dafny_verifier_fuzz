using System.Numerics;

namespace AST_new;

public abstract partial class Type : Node {
  public abstract string BaseName { get; }
  public List<Type> TypeArgs = new();

  public bool HasTypeArgs() => TypeArgs.Count > 0;

  public static readonly BoolType Bool = BoolType.Instance;
  public static readonly IntType Int = IntType.Instance;
  public static readonly StringType String = StringType.Instance;
}
