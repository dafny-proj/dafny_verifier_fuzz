namespace AST;

public partial class FunctionDecl : MemberDecl {
  public override string Name { get; protected set; }
  public Expression? Body { get; set; }
  public readonly List<Formal> Ins = new();
  public Formal? Result { get; set; }
  public Type ResultType { get; } // Should match Result.Type if Result != null.
  public Specification? Precondition { get; set; }
  public Specification? Postcondition { get; set; }
  public Specification? Reads { get; set; }
  public Specification? Decreases { get; set; }

  public bool HasBody() => Body != null;
  public bool HasNamedResult() => Result != null;
  public bool HasPrecondition() => Specification.HasUserDefinedSpec(Precondition);
  public bool HasPostcondition() => Specification.HasUserDefinedSpec(Postcondition);
  public bool HasReadsSpec() => Specification.HasUserDefinedSpec(Reads);
  public bool HasDecreasesSpec() => Specification.HasUserDefinedSpec(Decreases);
  public bool HasSpec() => HasPrecondition() || HasPostcondition()
                          || HasReadsSpec() || HasDecreasesSpec();

  public FunctionDecl(TopLevelDecl enclosingDecl, string name, Type resultType,
  Expression? body = null, IEnumerable<Formal>? ins = null,
  Formal? result = null, Specification? pre = null, Specification? post = null,
  Specification? reads = null, Specification? dec = null)
  : base(enclosingDecl) {
    Name = name;
    Body = body;
    if (ins != null) {
      Ins.AddRange(ins);
    }
    ResultType = resultType;
    Result = result;
    Precondition = pre;
    Postcondition = post;
    Reads = reads;
    Decreases = dec;
  }

  public static FunctionDecl Skeleton(TopLevelDecl enclosingDecl,
  string name, Type returnType)
    => new FunctionDecl(enclosingDecl, name, returnType);

  public override IEnumerable<Node> Children {
    get {
      foreach (var i in Ins) { yield return i; }
      if (Result != null) { yield return Result; }
      if (HasPrecondition()) { yield return Precondition!; }
      if (HasPostcondition()) { yield return Postcondition!; }
      if (HasReadsSpec()) { yield return Reads!; }
      if (HasDecreasesSpec()) { yield return Decreases!; }
      if (Body != null) { yield return Body; }
    }
  }
}
