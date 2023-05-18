namespace AST_new.Printer;

public partial class ASTPrinter {
  private void PrintExpression(Expression e) {
    switch (e) {
      case LiteralExpr le:
        PrintLiteralExpr(le);
        break;
      default:
        throw new UnsupportedNodePrintingException(e);
    }
  }

  private void PrintLiteralExpr(LiteralExpr le) {
    Write(le switch {
      BoolLiteralExpr b => b.Value ? "true" : "false",
      IntLiteralExpr i => i.Value.ToString(),
      StringLiteralExpr s => $"\"{s.Value}\"",
      _ => throw new UnsupportedNodePrintingException(le),
    });
  }
}
