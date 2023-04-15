namespace AST;

public class Printer {
  public TextWriter Wr;

  private int indent = 0;
  private void IncIndent(int n = 1) => indent += n;
  private void DecIndent(int n = 1) => indent -= n;
  private void Indent() => Wr.Write(new String(' ', indent * 2));

  private string Sep = "";
  private string NextSep = "";
  private void ResetSep(string init = "", string sep = ", ") {
    Sep = init;
    NextSep = sep;
  }
  private void WriteSep() {
    Wr.Write(Sep);
    Sep = NextSep;
  }

  public Printer(TextWriter wr) {
    Wr = wr;
  }

  public static string ProgramToString(Program program) {
    StringWriter wr = new StringWriter();
    new Printer(wr).PrintProgram(program);
    return wr.ToString();
  }

  public void PrintProgram(Program program) {
    PrintTopLevelDecls(program.DefaultModuleDef.TopLevelDecls);
  }

  private void PrintTopLevelDecls(List<TopLevelDecl> topLevelDecls) {
    foreach (TopLevelDecl decl in topLevelDecls) {
      PrintTopLevelDecl(decl);
    }
  }

  private void PrintTopLevelDecl(TopLevelDecl topLevelDecl) {
    switch (topLevelDecl) {
      case ClassDecl cd:
        PrintClassDecl(cd);
        break;
      default:
        throw new NotImplementedException();
    }
  }

  private void PrintClassDecl(ClassDecl cl) {
    if (cl.IsDefaultClass) {
      PrintMembers(cl.Members);
    } else {
      throw new NotImplementedException();
    }
  }

  private void PrintMembers(List<MemberDecl> mbs) {
    MemberDecl? prev = null;
    void PrintSeparator(MemberDecl? prev, MemberDecl cur) {
      if (prev is not null && cur is Method) {
        Wr.WriteLine();
      }
      // TODO: Handle other MemberDecl types
    }
    foreach (MemberDecl m in mbs) {
      PrintSeparator(prev, m);
      PrintMember(m);
      prev = m;
    }
  }

  private void PrintMember(MemberDecl mb) {
    switch (mb) {
      case Method m:
        PrintMethod(m);
        break;
      case Function f:
        PrintFunction(f);
        break;
      default:
        throw new NotImplementedException();
    }
  }

  private void PrintMethod(Method m) {
    Indent();
    string mKind = "method";
    string mName = m.Name;
    Wr.Write($"{mKind} {mName}");
    PrintFormals(m.Ins);
    if (m.Outs.Count > 0) {
      if (m.Ins.Count + m.Outs.Count <= 3) {
        Wr.Write(" returns ");
        PrintFormals(m.Outs);
      } else {
        Wr.WriteLine();
        IncIndent(2);
        Indent();
        Wr.Write($"returns ");
        PrintFormals(m.Outs);
        DecIndent(2);
      }
    }

    IncIndent();
    PrintSpec("requires", m.Req);
    PrintFrameSpec("modifies", m.Mod);
    PrintSpec("ensures", m.Ens);
    PrintDecreasesSpec(m.Decreases);
    DecIndent();

    Wr.WriteLine();
    Indent();
    PrintStatement(m.Body);
    Wr.WriteLine();
  }

  private void PrintSpec(string kind, List<AttributedExpression> aes) {
    foreach (AttributedExpression ae in aes) {
      Wr.WriteLine();
      Indent();
      Wr.Write($"{kind} ");
      PrintAttributedExpression(ae);
    }
  }

  private void PrintAttributedExpression(AttributedExpression ae) {
    PrintExpression(ae.E);
  }

  private void PrintFrameSpec(string kind, Specification<Dafny.FrameExpression, FrameExpression> frame) {
    if (frame.Expressions.Count <= 0) {
      return;
    }
    Wr.WriteLine();
    Indent();
    Wr.Write("{0} ");
    ResetSep();
    foreach (FrameExpression fe in frame.Expressions) {
      WriteSep();
      PrintExpression(fe.E);
    }
  }

  private void PrintDecreasesSpec(Specification<Dafny.Expression, Expression> decreases) {
    if (decreases.Expressions.Count <= 0) {
      return;
    }
    Wr.WriteLine();
    Indent();
    Wr.Write("decreases ");
    ResetSep();
    foreach (Expression e in decreases.Expressions) {
      WriteSep();
      PrintExpression(e);
    }
  }

  private void PrintStatement(Statement stmt) {
    switch (stmt) {
      case BlockStmt blockStmt:
        Wr.WriteLine("{");
        IncIndent();
        foreach (Statement s in blockStmt.Body) {
          Indent();
          PrintStatement(s);
          Wr.WriteLine();
        }
        DecIndent();
        Indent();
        Wr.Write("}");
        break;
      case UpdateStmt updateStmt:
        PrintUpdateStmt(updateStmt);
        break;
      case IfStmt ifStmt:
        PrintIfStmt(ifStmt);
        break;
      default:
        throw new NotImplementedException();
    }
  }

  private void PrintUpdateStmt(UpdateStmt stmt) {
    ResetSep();
    foreach (Expression lhs in stmt.Lhss) {
      WriteSep();
      PrintExpression(lhs);
    }
    if (stmt.Lhss.Count > 0) {
      Wr.Write(" := ");
    }
    ResetSep();
    foreach (AssignmentRhs rhs in stmt.Rhss) {
      WriteSep();
      PrintAssignRHS(rhs);
    }
    Wr.Write(";");
  }

  private void PrintAssignRHS(AssignmentRhs rhs) {
    switch (rhs) {
      case ExprRhs exprRhs:
        PrintExpression(exprRhs.Expr);
        break;
      default:
        throw new NotImplementedException();
    }
  }
  private void PrintExpression(Expression expr) {
    switch (expr) {
      case NameSegment ns:
        Wr.Write(ns.Name);
        break;
      case BinaryExpr binExpr:
        PrintExpression(binExpr.E0);
        Wr.Write($" {BinaryExpr.OpcodeString(binExpr.Op)} ");
        PrintExpression(binExpr.E1);
        break;
      case IntLiteralExpr intLitExpr:
        Wr.Write(intLitExpr.Value);
        break;
      case ParensExpression parensExpr:
        // Note that this deviates from the original Dafny printer which prints 
        // parentheses optimally instead of following the program
        Wr.Write("(");
        PrintExpression(parensExpr.E);
        Wr.Write(")");
        break;
      case NegationExpression negExpr:
        // Note that this deviates from the original Dafny printer which
        // potentially prints parentheses based on binding/context strengths 
        Wr.Write("-");
        PrintExpression(negExpr.E);
        break;
      default:
        throw new NotImplementedException();
    }
  }

  private void PrintFormals(List<Formal> fs) {
    Wr.Write("(");
    string sep = "";
    foreach (Formal f in fs) {
      Wr.Write(sep);
      sep = ", ";
      PrintFormal(f);
    }
    Wr.Write(")");
  }

  private void PrintFormal(Formal f) {
    Wr.Write($"{f.Name}: ");
    PrintType(f.Type);
  }

  private void PrintType(Type t) {
    Wr.Write(t.Name);
  }

  private void PrintFunction(Function f) {
    Indent();
    Wr.Write($"function {f.Name}");
    PrintFormals(f.Ins);
    Wr.Write(": ");
    if (f.Out != null) {
      Wr.Write("(");
      PrintFormal(f.Out);
      Wr.Write(")");
    } else {
      PrintType(f.OutType);
    }
    IncIndent();
    PrintSpec("requires", f.Req);
    PrintFrameSpec("reads", f.Reads);
    PrintSpec("ensures", f.Ens);
    PrintDecreasesSpec(f.Decreases);
    DecIndent();
    Wr.WriteLine();

    Indent();
    Wr.WriteLine("{");
    IncIndent();
    PrintExtendedExpr(f.Body);
    DecIndent();
    Indent();
    Wr.WriteLine("}");
  }

  private void PrintExtendedExpr(Expression expr) {
    switch (expr) {
      default:
        Indent();
        PrintExpression(expr);
        Wr.WriteLine();
        break;
    }
  }

  private void PrintIfStmt(IfStmt ifStmt) {
    Wr.Write("if ");
    if (ifStmt.Guard == null) {
      Wr.Write("*");
    } else {
      PrintExpression(ifStmt.Guard);
    }
    Wr.Write(" ");
    PrintStatement(ifStmt.Thn);

    if (ifStmt.Els == null) return;
    Wr.Write(" else ");
    PrintStatement(ifStmt.Els);
  }
}