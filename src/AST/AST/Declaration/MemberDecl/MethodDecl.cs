namespace AST;

public partial class MethodDecl : MemberDecl { }
public partial class ConstructorDecl : MethodDecl { }

public partial class MethodDecl : MemberDecl {
  public override string Name { get; protected set; }
  public readonly List<TypeParameterDecl> TypeParams = new();
  public BlockStmt? Body { get; set; }
  public readonly List<Formal> Ins = new();
  public readonly List<Formal> Outs = new();
  public Specification? Precondition { get; set; }
  public Specification? Postcondition { get; set; }
  public Specification? Modifies { get; set; }
  public Specification? Decreases { get; set; }

  public bool HasBody() => Body != null;
  public bool HasOuts() => Outs.Count > 0;
  public bool HasPrecondition() => Specification.HasUserDefinedSpec(Precondition);
  public bool HasPostcondition() => Specification.HasUserDefinedSpec(Postcondition);
  public bool HasModifiesSpec() => Specification.HasUserDefinedSpec(Modifies);
  public bool HasDecreasesSpec() => Specification.HasUserDefinedSpec(Decreases);
  public bool HasSpec() => HasPrecondition() || HasPostcondition()
                          || HasModifiesSpec() || HasDecreasesSpec();

  public MethodDecl(TopLevelDecl enclosingDecl, string name,
  IEnumerable<TypeParameterDecl>? typeParams = null, BlockStmt? body = null,
  IEnumerable<Formal>? ins = null, IEnumerable<Formal>? outs = null,
  Specification? pre = null, Specification? post = null,
  Specification? mod = null, Specification? dec = null)
  : base(enclosingDecl) {
    Name = name;
    if (typeParams != null) {
      TypeParams.AddRange(typeParams);
    }
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

  public override IEnumerable<Node> Children {
    get {
      foreach (var t in TypeParams) { yield return t; }
      foreach (var i in Ins) { yield return i; }
      foreach (var o in Outs) { yield return o; }
      if (HasPrecondition()) { yield return Precondition!; }
      if (HasPostcondition()) { yield return Postcondition!; }
      if (HasModifiesSpec()) { yield return Modifies!; }
      if (HasDecreasesSpec()) { yield return Decreases!; }
      if (Body != null) { yield return Body; }
    }
  }
}

public partial class ConstructorDecl : MethodDecl {
  public bool IsAnonymous() => Name == "_ctor";

  public ConstructorDecl(TopLevelDecl enclosingDecl, string name,
  IEnumerable<TypeParameterDecl>? typeParams = null, BlockStmt? body = null,
  IEnumerable<Formal>? ins = null, IEnumerable<Formal>? outs = null,
  Specification? pre = null, Specification? post = null,
  Specification? mod = null, Specification? dec = null)
  : base(enclosingDecl, name, typeParams, body, ins, outs, pre, post, mod, dec) { }

  new public static ConstructorDecl Skeleton(TopLevelDecl enclosingDecl, string name)
    => new ConstructorDecl(enclosingDecl, name);
}