namespace Fuzzer;

// The AST should not be modified during traversal. 
// Use this class to track AST modifications to apply post-traversal. 
public class ReplaceChildTask {
  public Node Parent;
  public Node OriginalChild;
  public Node NewChild;

  public ReplaceChildTask(Node parent, Node originalChild, Node newChild) {
    Parent = parent;
    OriginalChild = originalChild;
    NewChild = newChild;
  }

  public void Execute() {
    Parent.ReplaceChild(OriginalChild, NewChild);
  }
}
