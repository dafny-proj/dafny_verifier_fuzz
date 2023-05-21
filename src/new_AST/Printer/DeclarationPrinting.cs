namespace AST_new.Printer;

public partial class ASTPrinter {
  private void PrintTopLevelDecl(TopLevelDecl d) {
    switch (d) {
      case ModuleDecl md:
        PrintModuleDecl(md);
        break;
      case ClassDecl cd:
        PrintClassDecl(cd);
        break;
      case TypeParameterDecl tpd:
        PrintTypeParameterDecl(tpd);
        break;
      case TypeSynonymDecl tsd:
        PrintTypeSynonymDecl(tsd);
        break;
      case SubsetTypeDecl std:
        PrintSubsetTypeDecl(std);
        break;
      case DatatypeDecl dd:
        PrintDatatypeDecl(dd);
        break;
      default:
        throw new UnsupportedNodePrintingException(d);
    }
  }

  private void PrintMemberDecl(MemberDecl d) {
    switch (d) {
      case MethodDecl mtd:
        PrintMethodDecl(mtd);
        break;
      case FunctionDecl fd:
        PrintFunctionDecl(fd);
        break;
      case FieldDecl fd:
        PrintFieldDecl(fd);
        break;
      default:
        throw new UnsupportedNodePrintingException(d);
    }
  }

  private void PrintModuleDecl(ModuleDecl d) {
    PrintList<TopLevelDecl>(d.Decls, PrintTopLevelDecl, sep: "\n");
  }

  private void PrintClassDecl(ClassDecl d) {
    var named = d is not DefaultClassDecl;
    if (named) {
      WriteIndent();
      WriteLine($"class {d.Name} {{");
      IncIndent();
    }
    PrintMembers(d.Members);
    if (named) {
      DecIndent();
      WriteIndent();
      Write("}");
    }
    WriteLine();
  }

  private void PrintMethodHeader(MethodDecl d) {
    if (d is ConstructorDecl c) {
      Write("constructor");
      if (!c.IsAnonymous()) {
        Write($" {c.Name}");
      }
    } else {
      Write($"method {d.Name}");
    }
    PrintTypeParameters(d.TypeParams);
  }

  private void PrintMethodDecl(MethodDecl d) {
    WriteIndent();
    PrintMethodHeader(d);
    PrintFormals(d.Ins);
    if (d.HasOuts()) {
      Write(" returns ");
      PrintFormals(d.Outs);
    }
    bool printSpec = d.HasSpec();
    if (printSpec) {
      WriteLine();
      IncIndent();
      PrintSpecification(d.Precondition);
      PrintSpecification(d.Modifies);
      PrintSpecification(d.Postcondition);
      PrintSpecification(d.Decreases);
      DecIndent();
    }
    if (d.HasBody()) {
      if (printSpec) {
        WriteIndent();
      } else {
        Write(" ");
      }
      PrintBlockStmt(d.Body!);
    }
    WriteLine();
  }

  private void PrintFunctionDecl(FunctionDecl d) {
    WriteIndent();
    Write($"function {d.Name}");
    PrintFormals(d.Ins);
    Write(": ");
    if (d.HasNamedResult()) {
      Write("(");
      PrintFormal(d.Result!);
      Write(")");
    } else {
      PrintType(d.ResultType);
    }
    bool printSpec = d.HasSpec();
    if (printSpec) {
      WriteLine();
      IncIndent();
      PrintSpecification(d.Precondition);
      PrintSpecification(d.Reads);
      PrintSpecification(d.Postcondition);
      PrintSpecification(d.Decreases);
      DecIndent();
    }
    if (d.HasBody()) {
      if (printSpec) {
        WriteIndent();
      } else {
        Write(" ");
      }
      WriteLine("{");
      IncIndent();
      WriteIndent();
      PrintExpression(d.Body!);
      WriteLine();
      DecIndent();
      WriteIndent();
      Write("}");
    }
    WriteLine();
  }

  private void PrintTypeParameterDecl(TypeParameterDecl d) {
    Write(d.Name);
  }

  private void PrintTypeSynonymDecl(TypeSynonymDecl d) {
    WriteIndent();
    Write($"type {d.Name}");
    PrintTypeParameters(d.TypeParams);
    Write(" = ");
    PrintType(d.BaseType);
    WriteLine();
  }

  private void PrintSubsetTypeDecl(SubsetTypeDecl d) {
    WriteIndent();
    Write($"type {d.Name}");
    PrintTypeParameters(d.TypeParams);
    Write(" = ");
    PrintBoundVar(d.BaseIdent);
    Write(" | ");
    PrintExpression(d.Constraint);
    WriteLine();
  }

  private void PrintDatatypeDecl(DatatypeDecl d) {
    WriteIndent();
    Write($"datatype {d.Name}");
    PrintTypeParameters(d.TypeParams);
    Write(" = ");
    PrintList<DatatypeConstructorDecl>(
      d.Constructors, PrintDatatypeConstructorDecl, sep: " | ");
    if (d.HasMembers()) {
      WriteLine(" {");
      IncIndent();
      PrintMembers(d.Members);
      DecIndent();
      WriteIndent();
      Write("}");
    }
    WriteLine();
  }

  private void PrintDatatypeConstructorDecl(DatatypeConstructorDecl d) {
    Write(d.Name);
    if (d.HasParameters()) {
      PrintFormals(d.Parameters);
    }
  }

  private void PrintFieldDecl(FieldDecl d) {
    WriteIndent();
    Write($"var {d.Name}");
    Write(": ");
    PrintType(d.Type);
  }

}
