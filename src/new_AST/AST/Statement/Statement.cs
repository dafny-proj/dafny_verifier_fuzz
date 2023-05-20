namespace AST_new;

public abstract partial class Statement : Node {
  public List<string>? Labels { get; set; }
}
