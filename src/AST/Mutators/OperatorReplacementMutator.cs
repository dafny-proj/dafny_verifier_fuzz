namespace AST.Mutator;

public class OperatorReplacementMutation {
  public delegate void ApplyT();
  public ApplyT Apply;

  public OperatorReplacementMutation(BinaryExpr be) {
    Apply = () => { be.Op = GetRandomEquivOperand(be.Op); };
  }

  private BinaryExpr.Opcode GetRandomEquivOperand(BinaryExpr.Opcode op) {
    return BinaryExpr.GetEquivOperands(op).ElementAt(0);
  }

}

public class OperatorReplacementMutationFinder : ASTVisitor {

  public List<OperatorReplacementMutation> Mutations = new List<OperatorReplacementMutation>();

  public OperatorReplacementMutationFinder() { }

  protected override void VisitBinaryExpr(BinaryExpr be) {
    if (BinaryExpr.HasTypeEquivOperands(be.Op)) {
      Mutations.Add(new OperatorReplacementMutation(be));
    }
    base.VisitBinaryExpr(be);
  }
}