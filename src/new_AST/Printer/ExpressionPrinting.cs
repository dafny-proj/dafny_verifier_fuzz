namespace AST_new.Printer;

public partial class ASTPrinter {
  private void PrintExpression(Expression e) {
    switch (e) {
      case LiteralExpr le:
        PrintLiteralExpr(le);
        break;
      case IdentifierExpr ie:
        PrintIdentifierExpr(ie);
        break;
      case ParensExpr pe:
        PrintParensExpr(pe);
        break;
      case BinaryExpr be:
        PrintBinaryExpr(be);
        break;
      case UnaryExpr ue:
        PrintUnaryExpr(ue);
        break;
      case MemberSelectExpr me:
        PrintMemberSelectExpr(me);
        break;
      case CollectionSelectExpr cse:
        PrintCollectionSelectExpr(cse);
        break;
      case SeqDisplayExpr sqde:
        PrintCollectionDisplayExpr(sqde);
        break;
      case SetDisplayExpr stde:
        PrintCollectionDisplayExpr(stde);
        break;
      case MultiSetDisplayExpr msde:
        PrintCollectionDisplayExpr(msde);
        break;
      case MapDisplayExpr mpde:
        PrintCollectionDisplayExpr(mpde);
        break;
      case CollectionUpdateExpr cue:
        PrintCollectionUpdateExpr(cue);
        break;
      case DatatypeValueExpr dve:
        PrintDatatypeValueExpr(dve);
        break;
      case ThisExpr te:
        PrintThisExpr(te);
        break;
      default:
        throw new UnsupportedNodePrintingException(e);
    }
  }

  private void PrintLiteralExpr(LiteralExpr e) {
    Write(e switch {
      BoolLiteralExpr b => b.Value ? "true" : "false",
      IntLiteralExpr i => i.Value.ToString(),
      StringLiteralExpr s => $"\"{s.Value}\"",
      _ => throw new UnsupportedNodePrintingException(e),
    });
  }

  private void PrintIdentifierExpr(IdentifierExpr e) {
    Write(e.Name);
  }

  private void PrintParensExpr(ParensExpr e) {
    Write("(");
    PrintExpression(e.E);
    Write(")");
  }

  private void PrintBinaryExpr(BinaryExpr e) {
    PrintExpression(e.E0);
    Write($" {BinaryExpr.OpcodeAsString(e.Op)} ");
    PrintExpression(e.E1);
  }

  private void PrintUnaryExpr(UnaryExpr e) {
    if (e.Op is UnaryExpr.Opcode.Not) {
      Write("!");
      PrintExpression(e.E);
    } else if (e.Op is UnaryExpr.Opcode.Cardinality) {
      Write("|");
      PrintExpression(e.E);
      Write("|");
    } else if (e.Op is UnaryExpr.Opcode.Fresh) {
      Write("fresh(");
      PrintExpression(e.E);
      Write(")");
    } else if (e.Op is UnaryExpr.Opcode.Allocated) {
      Write("allocated(");
      PrintExpression(e.E);
      Write(")");
    } else {
      throw new UnsupportedNodePrintingException(e);
    }
  }

  private void PrintMemberSelectExpr(MemberSelectExpr e) {
    if (e.Receiver is not ImplicitThisExpr) {
      PrintExpression(e.Receiver);
      Write(".");
    }
    Write(e.MemberName);
  }

  private void PrintCollectionSelectExpr(CollectionSelectExpr e) {
    PrintExpression(e.Collection);
    Write("[");
    if (e is CollectionElementExpr ee) {
      PrintExpression(ee.Index);
    } else if (e is CollectionSliceExpr se) {
      if (se.Index0 != null) {
        PrintExpression(se.Index0);
      }
      Write("..");
      if (se.Index1 != null) {
        PrintExpression(se.Index1);
      }
    } else {
      throw new UnsupportedNodePrintingException(e);
    }
    Write("]");
  }

  private void PrintCollectionDisplayExpr(SeqDisplayExpr e) {
    Write("[");
    PrintExpressions(e.Elements);
    Write("]");
  }
  private void PrintCollectionDisplayExpr(SetDisplayExpr e) {
    Write("{");
    PrintExpressions(e.Elements);
    Write("}");
  }
  private void PrintCollectionDisplayExpr(MultiSetDisplayExpr e) {
    Write("multiset{");
    PrintExpressions(e.Elements);
    Write("}");
  }
  private void PrintCollectionDisplayExpr(MapDisplayExpr e) {
    Write("map[");
    PrintExpressionPairs(e.Elements);
    Write("]");
  }

  private void PrintCollectionUpdateExpr(CollectionUpdateExpr e) {
    PrintExpression(e.Collection);
    Write("[");
    PrintExpression(e.Index);
    Write(" := ");
    PrintExpression(e.Value);
    Write("]");
  }

  private void PrintDatatypeValueExpr(DatatypeValueExpr e) {
    Write($"{e.DatatypeName}.{e.ConstructorName}");
    if (e.HasArguments()) {
      Write("(");
      PrintExpressions(e.ConstructorArguments);
      Write(")");
    }
  }

  private void PrintThisExpr(ThisExpr te) {
    if (te is ImplicitThisExpr) return;
    Write("this");
  }
}
