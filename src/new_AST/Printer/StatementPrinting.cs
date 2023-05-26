namespace AST_new.Printer;

public partial class ASTPrinter {
  private void PrintStatement(Statement s) {
    if (s.Labels != null) {
      foreach (var label in s.Labels) {
        WriteLine($"label {label}:");
        WriteIndent();
      }
    }
    switch (s) {
      case BlockStmt bs:
        PrintBlockStmt(bs);
        break;
      case VarDeclStmt vs:
        PrintVarDeclStmt(vs);
        break;
      case UpdateStmt us:
        PrintUpdateStmt(us);
        break;
      case PrintStmt ps:
        PrintPrintStmt(ps);
        break;
      case ReturnStmt rs:
        PrintReturnStmt(rs);
        break;
      case IfStmt ifs:
        PrintIfStmt(ifs);
        break;
      case WhileLoopStmt ws:
        PrintWhileLoopStmt(ws);
        break;
      case ForLoopStmt ws:
        PrintForLoopStmt(ws);
        break;
      case BreakStmt bs:
        PrintBreakStmt(bs);
        break;
      case AssertStmt ats:
        PrintAssertStmt(ats);
        break;
      default:
        throw new UnsupportedNodePrintingException(s);
    }
  }

  // Indentation of first line of BlockStmt is handled by its parent.
  private void PrintBlockStmt(BlockStmt s) {
    WriteLine("{");
    IncIndent();
    foreach (var ss in s.Body) {
      WriteIndent();
      PrintStatement(ss);
      WriteLine();
    }
    DecIndent();
    WriteIndent();
    Write("}");
  }

  private void PrintPrintStmt(PrintStmt s) {
    Write("print ");
    PrintExpressions(s.Expressions);
    Write(";");
  }

  private void PrintVarDeclStmt(VarDeclStmt s) {
    Write("var ");
    PrintLocalVars(s.Vars);
    if (s.HasInitialiser()) {
      Write(" := ");
      PrintAssignmentRhss(s.Initialiser!.Rhss);
    }
    Write(";");
  }

  private void PrintUpdateStmt(UpdateStmt s) {
    if (s.HasLhs()) {
      PrintExpressions(s.Lhss);
      Write(" := ");
    }
    PrintAssignmentRhss(s.Rhss);
    Write(";");
  }

  private void PrintReturnStmt(ReturnStmt s) {
    Write("return");
    if (s.HasReturns()) {
      Write(" ");
      PrintAssignmentRhss(s.Returns!.Rhss);
    }
    Write(";");
  }

  private void PrintIfStmt(IfStmt s) {
    Write("if ");
    PrintGuard(s.Guard);
    Write(" ");
    PrintBlockStmt(s.Thn);
    if (s.HasElse()) {
      Write(" else ");
      PrintStatement(s.Els!);
    }
  }

  private void PrintGuard(Expression? guard) {
    if (guard == null) {
      Write("*");
    } else {
      PrintExpression(guard);
    }
  }

  private void PrintWhileLoopStmt(WhileLoopStmt s) {
    Write("while ");
    PrintGuard(s.Guard);
    PrintLoopStmtSpecAndBody(s);
  }

  private void PrintForLoopStmt(ForLoopStmt s) {
    Write($"for ");
    PrintBoundVar(s.LoopIndex);
    Write(" := ");
    PrintExpression(s.LoopStart);
    Write($" {(s.GoesUp ? "to" : "downto")} ");
    if (s.LoopEnd == null) {
      Write("*");
    } else {
      PrintExpression(s.LoopEnd);
    }
    PrintLoopStmtSpecAndBody(s);
  }

  private void PrintLoopStmtSpecAndBody(LoopStmt s) {
    bool printSpec = s.HasSpec();
    if (printSpec) {
      WriteLine();
      IncIndent();
      PrintSpecification(s.Invariants);
      PrintSpecification(s.Decreases);
      PrintSpecification(s.Modifies);
      DecIndent();
    }
    if (s.HasBody()) {
      if (printSpec) {
        WriteIndent();
      } else {
        Write(" ");
      }
      PrintBlockStmt(s.Body!);
    }
  }

  private void PrintBreakStmt(BreakStmt s) {
    for (int i = 0; i < s.Count - 1; i++) {
      Write("break ");
    }
    Write(s is BreakStmt ? "break" : "continue");
    if (s.HasTargetLabel()) {
      Write($" {s.TargetLabel}");
    }
    Write(";");
  }

  private void PrintAssertStmt(AssertStmt s) {
    Write("assert ");
    PrintExpression(s.Assertion);
    Write(";");
  }

}
