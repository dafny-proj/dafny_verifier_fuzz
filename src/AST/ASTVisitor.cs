namespace AST;

public abstract class ASTVisitor {

  public void VisitProgram(Program p) {
    p.DefaultModuleDef.TopLevelDecls.ForEach(VisitTopLevelDecl);
  }

  protected void VisitTopLevelDecl(TopLevelDecl td) {
    switch (td) {
      case LiteralModuleDecl ld:
        VisitLiteralModuleDecl(ld);
        break;
      case ClassDecl cd:
        VisitClassDecl(cd);
        break;
      default:
        throw new NotImplementedException();
    }
  }

  protected void VisitLiteralModuleDecl(LiteralModuleDecl ld) {
    // TODO: handle abstract modules
    ld.ModuleDef.TopLevelDecls.ForEach(VisitTopLevelDecl);
  }

  protected void VisitClassDecl(ClassDecl cd) {
    cd.Members.ForEach(VisitMemberDecl);
  }

  protected void VisitMemberDecl(MemberDecl md) {
    switch (md) {
      case Method m:
        VisitMethod(m);
        break;
      default:
        throw new NotImplementedException();
    }

  }

  protected void VisitMethod(Method m) {
    VisitBlockStmt(m.Body);
    VisitSpecification(m.Decreases);
    m.Ins.ForEach(VisitFormal);
    m.Outs.ForEach(VisitFormal);
  }

  protected void VisitStatement(Statement s) {
    switch (s) {
      case BlockStmt bs:
        VisitBlockStmt(bs);
        break;
      case UpdateStmt us:
        VisitUpdateStmt(us);
        break;
      default:
        throw new NotImplementedException();
    }
  }

  protected void VisitBlockStmt(BlockStmt bs) {
    bs.Body.ForEach(VisitStatement);
  }

  protected void VisitUpdateStmt(UpdateStmt us) {
    us.Lhss.ForEach(VisitExpression);
    us.Rhss.ForEach(VisitAssignRhs);
  }

  protected void VisitAssignRhs(AssignmentRhs ar) {
    switch (ar) {
      case ExprRhs er:
        VisitExprRhs(er);
        break;
      default:
        throw new NotImplementedException();
    }
  }

  protected void VisitExprRhs(ExprRhs er) {
    VisitExpression(er.Expr);
  }

  protected void VisitSpecification(Specification<Dafny.Expression, Expression> d) {
    d.Expressions.ForEach(VisitExpression);
  }

  protected void VisitExpression(Expression e) {
    switch (e) {
      case BinaryExpr be:
        VisitBinaryExpr(be);
        break;
      case NameSegment ns:
        VisitNameSegment(ns);
        break;
      default:
        throw new NotImplementedException();
    }
  }

  protected virtual void VisitBinaryExpr(BinaryExpr be)
  {
    VisitExpression(be.E0);
    VisitExpression(be.E1);
  }

  protected void VisitNameSegment(NameSegment ns) {}

  protected void VisitFormal(Formal f) {
    VisitType(f.Type);
  }

  protected void VisitType(Type t) { }
}