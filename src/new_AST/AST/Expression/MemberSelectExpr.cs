namespace AST_new;

public partial class MemberSelectExpr : Expression {
  public Expression Receiver { get; }
  public MemberDecl Member { get; }
  public string MemberName => Member.Name;

  public MemberSelectExpr(Expression receiver, MemberDecl member) {
    Receiver = receiver;
    Member = member;
  }
}
