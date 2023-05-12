namespace Fuzzer;

public class ParentMap : IASTVisitor {
  private Dictionary<Node, Node> ChildToParent = new();

  public ParentMap(Node n) {
    VisitNode(n);
  }

  public void VisitNode(Node n) {
    foreach (var c in n.Children) {
      AddParent(c, n);
      VisitNode(c);
    }
  }

  public Node GetParent(Node child) {
    if (!ChildToParent.ContainsKey(child)) {
      throw new ArgumentException($"Cannot find parent for `{child}`.");
    }
    return ChildToParent[child];
  }

  private void AddParent(Node child, Node parent) {
    if (child is Type) return;
    if (ChildToParent.ContainsKey(child)) {
      throw new ArgumentException($"Found multiple parents for `{child}`.");
    }
    ChildToParent.Add(child, parent);
  }

}