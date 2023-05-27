namespace AST;

public partial class MemberSelectExpr : Expression {
  public Expression Receiver { get; set; }
  public MemberDecl Member { get; }
  public string MemberName => Member.Name;

  public MemberSelectExpr(Expression receiver, MemberDecl member) {
    Receiver = receiver;
    Member = member;
  }

  public override IEnumerable<Node> Children => new[] { Receiver };
}
