namespace AST_new.Printer;

public partial class ASTPrinter {
  private void PrintTopLevelDecl(TopLevelDecl tld) {
    switch (tld) {
      case ModuleDecl md:
        PrintModule(md);
        break;
      case ClassDecl cd:
        PrintClassDecl(cd);
        break;
      default:
        throw new UnsupportedNodePrintingException(tld);
    }
  }

  private void PrintModule(ModuleDecl md) {
    md.Decls.ForEach(PrintTopLevelDecl);
  }

  private void PrintClassDecl(ClassDecl cd) {
    var named = cd is not DefaultClassDecl;
    if (named) {
      WriteIndent();
      WriteLine($"class {cd.Name} {{");
      IncIndent();
    }
    cd.Members.ForEach(PrintMemberDecl);
    if (named) {
      DecIndent();
      WriteIndent();
      WriteLine("}");
    }
  }

  private void PrintMemberDecl(MemberDecl md) {
    switch (md) {
      case MethodDecl mtd:
        PrintMethod(mtd);
        break;
      default:
        throw new UnsupportedNodePrintingException(md);
    }
  }

  private void PrintMethod(MethodDecl mtd) {
    WriteIndent();
    Write($"method {mtd.Name}");
    PrintFormals(mtd.Ins);
    if (mtd.HasOuts()) {
      Write(" returns ");
      PrintFormals(mtd.Outs);
    }
    WriteLine();

    IncIndent();
    if (mtd.HasPrecondition()) {
      PrintSpecification(mtd.Precondition!);
    }
    if (mtd.HasModifiesSpec()) {
      PrintSpecification(mtd.Modifies!);
    }
    if (mtd.HasPostcondition()) {
      PrintSpecification(mtd.Postcondition!);
    }
    if (mtd.HasDecreasesSpec()) {
      PrintSpecification(mtd.Decreases!);
    }
    DecIndent();

    if (mtd.HasBody()) {
      PrintBlockStmt(mtd.Body!);
    }

    WriteLine();
  }

}
