namespace AST;

public class MemberSelectExpr
: Expression, ConstructableFromDafny<Dafny.MemberSelectExpr, MemberSelectExpr> {
  public Expression Receiver { get; set; }
  public bool ReceiverIsImplicit { get; }
  public string MemberName { get; set; }
  public MemberDecl Member { get; set; } // TODO: Is this field needed?

  private MemberSelectExpr(Dafny.MemberSelectExpr mse) {
    Receiver = Expression.FromDafny(mse.Obj);
    // TODO: is it better to check the type of the receiver for deducing implicitness?
    ReceiverIsImplicit = mse.Obj.IsImplicit;
    MemberName = mse.MemberName;
    Member = MemberDecl.FromDafny(mse.Member);
  }

  public static MemberSelectExpr FromDafny(Dafny.MemberSelectExpr dafnyNode) {
    return new MemberSelectExpr(dafnyNode);
  }
}
