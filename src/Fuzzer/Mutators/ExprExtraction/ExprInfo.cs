namespace Fuzzer;

public class ExprInfo {
  public Expression E;
  public Node Parent;
  public ModuleDecl EnclosingModule;
  public ExprInfo(Expression e, Node parent, ModuleDecl enclosingModule) {
    E = e;
    Parent = parent;
    EnclosingModule = enclosingModule;
  }
}

public class ExprInfoBuilder {
  private List<ExprInfo> exprInfos = new();
  private Stack<ModuleDecl> modules = new();
  private Stack<Node> parents = new();
  private Node? current;

  public static List<ExprInfo> FindExprInfo(Node n) {
    var b = new ExprInfoBuilder();
    b.VisitNode(n);
    return b.exprInfos;
  }

  private ModuleDecl GetModule() => modules.Peek();
  private Node GetParent() => parents.Peek();
  private void EnterNode(Node n) {
    if (n is ModuleDecl m) { modules.Push(m); }
    if (current != null) { parents.Push(current); }
    current = n;
  }
  private void ExitNode(Node n) {
    if (n is ModuleDecl m) { modules.Pop(); }
    current = parents.Count == 0 ? null : parents.Pop();
  }
  private void VisitNode(Node n) {
    EnterNode(n);
    if (n is Expression e) {
      exprInfos.Add(new ExprInfo(e, GetParent(), GetModule()));
    }
    foreach (var c in n.Children) { VisitNode(c); }
    ExitNode(n);
  }
}
