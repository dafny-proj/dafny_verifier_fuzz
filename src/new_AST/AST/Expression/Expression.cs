namespace AST_new;

public abstract partial class Expression : Node {
  public virtual Type Type {
    get => throw new UnsupportedNodeOperationException(this, 
      "expression type retrieval");
    protected set => Type = value;
  }

}