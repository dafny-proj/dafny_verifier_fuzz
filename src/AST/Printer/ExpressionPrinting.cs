namespace AST.Printer;

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
      case SetComprehensionExpr sce:
        PrintComprehensionExpr(sce);
        break;
      case MapComprehensionExpr mce:
        PrintComprehensionExpr(mce);
        break;
      case CollectionUpdateExpr cue:
        PrintCollectionUpdateExpr(cue);
        break;
      case MultiSetConstructionExpr me:
        PrintMultiSetConstructionExpr(me);
        break;
      case SeqConstructionExpr se:
        PrintSeqConstructionExpr(se);
        break;
      case DatatypeValueExpr dve:
        PrintDatatypeValueExpr(dve);
        break;
      case ThisExpr te:
        PrintThisExpr(te);
        break;
      case WildcardExpr we:
        PrintWildcardExpr(we);
        break;
      case FunctionCallExpr fe:
        PrintFunctionCallExpr(fe);
        break;
      case StaticReceiverExpr se:
        PrintStaticReceiverExpr(se);
        break;
      case ITEExpr ie:
        PrintITEExpr(ie);
        break;
      case LetExpr le:
        PrintLetExpr(le);
        break;
      case QuantifierExpr qe:
        PrintQuantifierExpr(qe);
        break;
      case MatchExpr me:
        PrintMatchExpr(me);
        break;
      case LambdaExpr le:
        PrintLambdaExpr(le);
        break;
      case DatatypeUpdateExpr de:
        PrintDatatypeUpdateExpr(de);
        break;
      case TypeUnaryExpr te:
        PrintTypeUnaryExpr(te);
        break;
      default:
        throw new UnsupportedNodePrintingException(e);
    }
  }

  private void PrintLiteralExpr(LiteralExpr e) {
    Write(e switch {
      BoolLiteralExpr b => b.Value ? "true" : "false",
      CharLiteralExpr c => $"'{c.Value}'",
      IntLiteralExpr i => i.Value.ToString(),
      RealLiteralExpr r => r.Value,
      StringLiteralExpr s => $"\"{s.Value}\"",
      NullLiteralExpr => "null",
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
    var separator = "";
    if (e.Receiver is not (ImplicitThisExpr or ImplicitStaticReceiverExpr)) {
      PrintExpression(e.Receiver);
      separator = ".";
    }
    if (e is FrameFieldExpr) { separator = "`"; }
    Write($"{separator}{e.MemberName}");
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

  private void PrintComprehensionExpr(SetComprehensionExpr e) {
    Write("set ");
    PrintQuantifierDomain(e.QuantifierDomain);
    if (e.Value != null) {
      Write(" :: ");
      PrintExpression(e.Value);
    }
  }

  private void PrintComprehensionExpr(MapComprehensionExpr e) {
    Write("map ");
    PrintQuantifierDomain(e.QuantifierDomain);
    Write(" :: ");
    if (e.Key != null) {
      PrintExpression(e.Key);
      Write(" := ");
    }
    PrintExpression(e.Value);
  }

  private void PrintCollectionUpdateExpr(CollectionUpdateExpr e) {
    PrintExpression(e.Collection);
    Write("[");
    PrintExpression(e.Index);
    Write(" := ");
    PrintExpression(e.Value);
    Write("]");
  }

  private void PrintMultiSetConstructionExpr(MultiSetConstructionExpr e) {
    Write("multiset(");
    PrintExpression(e.E);
    Write(")");
  }

  private void PrintSeqConstructionExpr(SeqConstructionExpr e) {
    Write("seq(");
    PrintExpression(e.Count);
    Write(", ");
    PrintExpression(e.Initialiser);
    Write(")");
  }

  private void PrintDatatypeValueExpr(DatatypeValueExpr e) {
    var isTuple = e.Constructor.EnclosingDecl is TupleTypeDecl;
    if (!isTuple) { Write($"{e.DatatypeName}.{e.ConstructorName}"); }
    if (e.HasArguments() || isTuple) {
      Write("(");
      PrintExpressions(e.ConstructorArguments);
      Write(")");
    }
  }

  private void PrintThisExpr(ThisExpr e) {
    if (e is ImplicitThisExpr) return;
    Write("this");
  }

  private void PrintWildcardExpr(WildcardExpr e) {
    Write("*");
  }

  private void PrintFunctionCallExpr(FunctionCallExpr e) {
    PrintExpression(e.Callee);
    Write("(");
    PrintExpressions(e.Arguments);
    Write(")");
  }

  private void PrintStaticReceiverExpr(StaticReceiverExpr e) {
    if (e is ImplicitStaticReceiverExpr) { return; }
    Write(e.Decl.Name);
  }

  private void PrintITEExpr(ITEExpr e) {
    Write("if ");
    PrintExpression(e.Guard);
    Write(" then ");
    PrintExpression(e.Thn);
    Write(" else ");
    PrintExpression(e.Els);
  }

  private void PrintLetExpr(LetExpr e) {
    var lhss = e.Vars.Select(v => v.Key);
    var rhss = e.Vars.Select(v => v.Value);
    Write("var ");
    PrintBoundVars(lhss);
    Write(" := ");
    PrintExpressions(rhss);
    Write("; ");
    PrintExpression(e.Body);
  }

  private void PrintQuantifierExpr(QuantifierExpr e) {
    Write(e.Quantifier + " ");
    PrintQuantifierDomain(e.QuantifierDomain);
    Write(" :: ");
    PrintExpression(e.Term);
  }

  private void PrintMatchExpr(MatchExpr e) {
    Write("match ");
    PrintExpression(e.Selector);
    WriteLine(" {");
    IncIndent();
    foreach (var c in e.Cases) {
      WriteIndent();
      PrintMatchExprCase(c);
      WriteLine();
    }
    DecIndent();
    WriteIndent();
    Write("}");
  }

  private void PrintLambdaExpr(LambdaExpr e) {
    PrintList<BoundVar>(e.Params, PrintBoundVar, start: "(", end: ")");
    PrintSpecification(e.Precondition, oneLine: true);
    PrintSpecification(e.ReadFrame, oneLine: true);
    Write(" => ");
    PrintExpression(e.Result);
  }

  private void PrintDatatypeUpdateExpr(DatatypeUpdateExpr e) {
    PrintExpression(e.DatatypeValue);
    PrintList<DatatypeUpdatePair>(e.Updates, PrintDatatypeUpdatePair,
      start: ".(", end: ")");
  }

  private void PrintTypeUnaryExpr(TypeUnaryExpr e) {
    PrintExpression(e.E);
    Write($" {e.OpStr} ");
    PrintType(e.T);
  }

}
