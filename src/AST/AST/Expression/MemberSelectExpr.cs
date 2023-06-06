namespace AST;

public partial class MemberSelectExpr : Expression { }
public partial class FrameFieldExpr : MemberSelectExpr { }

public partial class MemberSelectExpr : Expression {
  public Expression Receiver { get; set; }
  public MemberDecl Member { get; }
  public string MemberName => Member.Name;

  public MemberSelectExpr(Expression receiver, MemberDecl member,
  Type? type = null) {
    Receiver = receiver;
    Member = member;
    if (type != null) { Type = type; }
  }

  public override IEnumerable<Node> Children => new[] { Receiver };
}

public partial class FrameFieldExpr : MemberSelectExpr {
  public FrameFieldExpr(Expression receiver, FieldDecl member)
  : base(receiver, member) { }

  public override Type Type => ((FieldDecl)Member).Type;
}
