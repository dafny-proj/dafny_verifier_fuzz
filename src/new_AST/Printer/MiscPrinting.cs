namespace AST_new.Printer;

public partial class ASTPrinter {
  private void PrintFormals(List<Formal> fs) {
    Write("(");
    ResetSeparator();
    foreach (var f in fs) {
      WriteSeparator();
      PrintFormal(f);
    }
    Write(")");
  }

  private void PrintFormal(Formal f) {
    Write($"{f.Name}: ");
    PrintType(f.Type);
    if (f.HasDefaultValue()) {
      Write(" := ");
      PrintExpression(f.DefaultValue!);
    }
  }

  private void PrintSpecification(Specification s) {
    throw new UnsupportedNodePrintingException(s);
  }

}
