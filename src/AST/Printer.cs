namespace AST;

public class Printer
{
  public TextWriter Wr;

  private int indent = 0;
  private void IncIndent(int n = 1) => indent += n;
  private void DecIndent(int n = 1) => indent -= n;
  private void Indent() => Wr.Write(new String(' ', indent * 2));

  private string Sep = "";
  private string NextSep = "";
  private void ResetSep(string init = "", string sep = ", ")
  {
    Sep = init;
    NextSep = sep;
  }
  private void WriteSep()
  {
    Wr.Write(Sep);
    Sep = NextSep;
  }

  public Printer(TextWriter wr)
  {
    Wr = wr;
  }

  public static string ProgramToString(Program program) {
    StringWriter wr = new StringWriter();
    new Printer(wr).PrintProgram(program);
    return wr.ToString();
  }

  public void PrintProgram(Program program)
  {
    PrintTopLevelDecls(program.DefaultModuleDef.TopLevelDecls);
  }

  private void PrintTopLevelDecls(List<TopLevelDecl> topLevelDecls)
  {
    foreach (TopLevelDecl decl in topLevelDecls)
    {
      PrintTopLevelDecl(decl);
    }
  }

  private void PrintTopLevelDecl(TopLevelDecl topLevelDecl)
  {
    switch (topLevelDecl)
    {
      case ClassDecl cd:
        PrintClassDecl(cd);
        break;
      default:
        throw new NotImplementedException();
    }
  }

  private void PrintClassDecl(ClassDecl cl)
  {
    if (cl.IsDefaultClass)
    {
      PrintMembers(cl.Members);
    }
    else
    {
      throw new NotImplementedException();
    }
  }

  private void PrintMembers(List<MemberDecl> mbs)
  {
    MemberDecl? prev = null;
    void PrintSeparator(MemberDecl? prev, MemberDecl cur)
    {
      if (prev is not null && cur is Method)
      {
        Wr.WriteLine();
      }
      // TODO: Handle other MemberDecl types
    }
    foreach (MemberDecl m in mbs)
    {
      PrintSeparator(prev, m);
      PrintMember(m);
      prev = m;
    }
  }

  private void PrintMember(MemberDecl mb)
  {
    switch (mb)
    {
      case Method m:
        PrintMethod(m);
        break;
      default:
        throw new NotImplementedException();
    }
  }

  private void PrintMethod(Method m)
  {
    Indent();
    string mKind = "method";
    string mName = m.Name;
    Wr.Write($"{mKind} {mName}");
    PrintFormals(m.Ins);
    if (m.Outs.Count > 0)
    {
      if (m.Ins.Count + m.Outs.Count <= 3)
      {
        Wr.Write(" returns ");
        PrintFormals(m.Outs);
      }
      else
      {
        Wr.WriteLine();
        IncIndent(2);
        Indent();
        Wr.Write($"returns ");
        PrintFormals(m.Outs);
        DecIndent(2);
      }
    }
    Wr.WriteLine();
    Indent();
    PrintStatement(m.Body);
    Wr.WriteLine();
  }

  private void PrintStatement(Statement stmt)
  {
    switch (stmt)
    {
      case BlockStmt blockStmt:
        Wr.WriteLine("{");
        IncIndent();
        foreach (Statement s in blockStmt.Body)
        {
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
      default:
        throw new NotImplementedException();
    }
  }

  private void PrintUpdateStmt(UpdateStmt stmt)
  {
    ResetSep();
    foreach (Expression lhs in stmt.Lhss)
    {
      WriteSep();
      PrintExpression(lhs);
    }
    if (stmt.Lhss.Count > 0)
    {
      Wr.Write(" := ");
    }
    ResetSep();
    foreach (AssignmentRhs rhs in stmt.Rhss)
    {
      WriteSep();
      PrintAssignRHS(rhs);
    }
    Wr.Write(";");
  }

  private void PrintAssignRHS(AssignmentRhs rhs)
  {
    switch (rhs) {
      case ExprRhs exprRhs:
        PrintExpression(exprRhs.Expr);
        break;
      default:
        throw new NotImplementedException();
    }
  }
  private void PrintExpression(Expression expr)
  {
    switch (expr) {
      case NameSegment ns:
        Wr.Write(ns.Name);
        break;
      case BinaryExpr binExpr:
        PrintExpression(binExpr.E0);
        Wr.Write($" {BinaryExpr.OpcodeString(binExpr.Op)} ");
        PrintExpression(binExpr.E1);
        break;
      default:
        throw new NotImplementedException();
    }
  }

  private void PrintFormals(List<Formal> fs)
  {
    Wr.Write("(");
    string sep = "";
    foreach (Formal f in fs)
    {
      Wr.Write(sep);
      sep = ", ";
      PrintFormal(f);
    }
    Wr.Write(")");
  }

  private void PrintFormal(Formal f)
  {
    Wr.Write($"{f.Name}: ");
    PrintType(f.Type);
  }

  private void PrintType(Type t)
  {
    Wr.Write(t.Name);
  }
}