namespace AST.Printer;

public partial class ASTPrinter {
  private void PrintAssignmentRhs(AssignmentRhs a) {
    switch (a) {
      case ExprRhs er:
        PrintExprRhs(er);
        break;
      case MethodCallRhs mr:
        PrintMethodCallRhs(mr);
        break;
      case NewArrayRhs nar:
        PrintNewArrayRhs(nar);
        break;
      case NewObjectRhs nor:
        PrintNewObjectRhs(nor);
        break;
      default:
        throw new UnsupportedNodePrintingException(a);
    }
  }

  private void PrintExprRhs(ExprRhs a) {
    PrintExpression(a.E);
  }

  private void PrintMethodCallRhs(MethodCallRhs a) {
    PrintExpression(a.Callee);
    Write("(");
    PrintExpressions(a.Arguments);
    Write(")");
  }

  private void PrintNewArrayRhs(NewArrayRhs a) {
    Write("new ");
    PrintType(a.ElementType);
    Write("[");
    PrintExpressions(a.Dimensions);
    Write("]");
    if (a is NewArrayWithElementInitialiserRhs ae) {
      Write("(");
      PrintExpression(ae.ElementInitialiser);
      Write(")");
    } else if (a is NewArrayWithListInitialiserRhs al) {
      Write("[");
      PrintExpressions(al.ListInitialiser);
      Write("]");
    }
  }

  private void PrintNewObjectRhs(NewObjectRhs a) {
    Write("new ");
    PrintType(a.ObjectType);
    if (a is NewObjectWithConstructorRhs ac) {
      if (!ac.Constructor.IsAnonymous()) {
        Write($".{ac.Constructor.Name}");
      }
      Write("(");
      PrintExpressions(ac.ConstructorArguments);
      Write(")");
    }
  }
}
