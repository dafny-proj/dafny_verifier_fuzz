namespace AST_new.Printer;

public partial class ASTPrinter {
  private void PrintStatement(Statement s) {
    switch (s) {
      case BlockStmt bs:
        PrintBlockStmt(bs);
        break;
      case PrintStmt ps:
        PrintPrintStmt(ps);
        break;
      default:
        throw new UnsupportedNodePrintingException(s);
    }
  }

  private void PrintBlockStmt(BlockStmt bs) {
    WriteIndent();
    WriteLine("{");
    IncIndent();
    foreach (var s in bs.Body) {
      PrintStatement(s);
    }
    DecIndent();
    WriteIndent();
    WriteLine("}");
  }

  private void PrintPrintStmt(PrintStmt ps) {
    WriteIndent();
    Write("print ");
    ResetSeparator();
    foreach (var e in ps.Expressions) {
      WriteSeparator();
      PrintExpression(e);
    }
    WriteLine(";");
  }

}
