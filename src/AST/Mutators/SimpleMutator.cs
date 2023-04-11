namespace AST.Mutator;

public class SimpleMutation {
  public delegate void ApplyT();
  public ApplyT Apply;

  public SimpleMutation(BinaryExpr be) {
    Apply = () => { be.Op = GetRandomEquivOperand(be.Op); };
  }

  private BinaryExpr.Opcode GetRandomEquivOperand(BinaryExpr.Opcode op) {
    return BinaryExpr.GetEquivOperands(op).ElementAt(0);
  }

}

public class SimpleMutationFinder : ASTVisitor {

  public List<SimpleMutation> Mutations = new List<SimpleMutation>();

  public SimpleMutationFinder() { }

  protected override void VisitBinaryExpr(BinaryExpr be) {
    if (BinaryExpr.HasTypeEquivOperands(be.Op)) {
      Mutations.Add(new SimpleMutation(be));
    }
  }
}