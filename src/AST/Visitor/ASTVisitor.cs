namespace AST;

public abstract class ASTVisitor : IASTVisitor {
  public void VisitChildren(Node n) {
    foreach (var c in n.Children) {
      VisitNode(c);
    }
  }

  public virtual void VisitNode(Node n) {
    switch (n) {
      case Program p:
        VisitProgram(p);
        break;
      case Declaration d:
        VisitDecl(d);
        break;
      case Statement s:
        VisitStmt(s);
        break;
      case Expression e:
        VisitExpr(e);
        break;
      case AssignmentRhs ar:
        VisitAssignRhs(ar);
        break;
      case Type t:
        VisitType(t);
        break;
      default:
        VisitChildren(n);
        return;
    }
  }

  public virtual void VisitProgram(Program p) {
    VisitChildren(p);
  }
  public virtual void VisitDecl(Declaration d) {
    VisitChildren(d);
  }
  public virtual void VisitStmt(Statement s) {
    VisitChildren(s);
  }
  public virtual void VisitExpr(Expression e) {
    VisitChildren(e);
  }
  public virtual void VisitAssignRhs(AssignmentRhs ar) {
    VisitChildren(ar);
  }
  public virtual void VisitType(Type t) {
    VisitChildren(t);
  }
}