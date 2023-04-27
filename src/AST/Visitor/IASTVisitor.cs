namespace AST;

// Add hooks as needed
public interface IASTVisitor {
  public void VisitNode(Node n);
  public void VisitProgram(Program p);
  public void VisitDecl(Declaration d);
  public void VisitStmt(Statement s);
  public void VisitExpr(Expression e);
  public void VisitAssignRhs(AssignmentRhs ar);
  public void VisitType(Type t);

}