namespace AST;

public abstract partial class MemberDecl : Declaration {
  public TopLevelDecl EnclosingDecl { get; }

  public MemberDecl(TopLevelDecl enclosingDecl) {
    EnclosingDecl = enclosingDecl;
  }
}
