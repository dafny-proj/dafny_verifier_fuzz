namespace AST_new;

public partial class MethodDecl : MemberDecl {
  public string Name { get; }
  public BlockStmt? Body { get; set; }
  public readonly List<Formal> Ins = new();
  public readonly List<Formal> Outs = new();
  public Specification? Precondition { get; set; }
  public Specification? Postcondition { get; set; }
  public Specification? Modifies { get; set; }
  public Specification? Decreases { get; set; }

  public bool HasBody() => Body != null;
  public bool HasOuts() => Outs.Count > 0;
  public bool HasPrecondition() => Precondition != null;
  public bool HasPostcondition() => Postcondition != null;
  public bool HasModifiesSpec() => Modifies != null;
  public bool HasDecreasesSpec() => Decreases != null;

  public MethodDecl(TopLevelDecl enclosingDecl, string name,
  BlockStmt? body = null, IEnumerable<Formal>? ins = null,
  IEnumerable<Formal>? outs = null, Specification? pre = null,
  Specification? post = null, Specification? mod = null, Specification? dec = null)
  : base(enclosingDecl) {
    Name = name;
    Body = body;
    if (ins != null) {
      Ins.AddRange(ins);
    }
    if (outs != null) {
      Outs.AddRange(outs);
    }
    Precondition = pre;
    Postcondition = post;
    Modifies = mod;
    Decreases = dec;
  }

  public static MethodDecl Skeleton(TopLevelDecl enclosingDecl, string name)
    => new MethodDecl(enclosingDecl, name);
}