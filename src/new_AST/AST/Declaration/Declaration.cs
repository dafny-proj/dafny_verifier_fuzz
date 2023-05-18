namespace AST_new;

public abstract partial class Declaration : Node {
  public virtual string Name {
    get => throw new UnsupportedNodeOperationException(
      this, "declaration naming");
    protected set => Name = value;
  }
}
