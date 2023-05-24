namespace AST_new;

public abstract partial class Statement : Node {
  public List<string>? Labels { get; set; }

  public void AddLabel(string label) {
    if (Labels == null) {
      Labels = new();
    }
    Labels.Add(label);
  }
}
