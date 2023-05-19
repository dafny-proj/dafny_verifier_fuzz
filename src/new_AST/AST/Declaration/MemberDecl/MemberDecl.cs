namespace AST_new;

public abstract partial class MemberDecl : Declaration { }
public partial class MethodDecl : MemberDecl { }

public abstract partial class MemberDecl : Declaration {
  public TopLevelDecl EnclosingDecl { get; }

  public MemberDecl(TopLevelDecl enclosingDecl) {
    EnclosingDecl = enclosingDecl;
  }
}