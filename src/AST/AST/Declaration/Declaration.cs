namespace AST;

public abstract partial class Declaration : Node {
  public virtual string Name {
    get => throw new UnsupportedASTOperationException(
      this, "declaration naming");
    protected set => Name = value;
  }
}
