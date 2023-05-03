namespace Fuzzer;

public class LoopRewriteMutation {
  public ILoopParser? Parser { get; set; }
  public ILoopWriter? Writer { get; set; }
  public Node Parent { get; set; }
  public Node OriginalLoop { get; set; }

  public LoopRewriteMutation(Node parent, Node originalLoop) {
    Parent = parent;
    OriginalLoop = originalLoop;
  }

  public void Apply() {
    if (Parser == null || Writer == null) return;
    throw new NotImplementedException();
  }

  public void RewriteLoop(Node RewrittenLoop) {
    Parent.ReplaceChild(OriginalLoop, RewrittenLoop);
  }


}
