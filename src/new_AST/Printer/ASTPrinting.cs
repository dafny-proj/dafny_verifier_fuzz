namespace AST_new.Printer;

public partial class ASTPrinter {
  private TextWriter _wr { get; }
  private void Write(string s) => _wr.Write(s);
  private void WriteLine(string? s = null) => _wr.WriteLine(s);

  private int _indent = 0;
  private void IncIndent(int n = 1) => _indent += n;
  private void DecIndent(int n = 1) => _indent -= n;
  private void WriteIndent() => Write(new String(' ', _indent * 2));

  private string _sep = "";
  private string _nextSep = "";
  private void ResetSeparator(string init = "", string sep = ", ") {
    _sep = init;
    _nextSep = sep;
  }
  private void WriteSeparator() {
    Write(_sep);
    _sep = _nextSep;
  }

  public ASTPrinter(TextWriter wr) {
    _wr = wr;
  }

  public static string NodeToString(Node n) {
    var wr = new StringWriter();
    var pr = new ASTPrinter(wr);
    pr.PrintNode(n);
    return wr.ToString();
  }

  private void PrintNode(Node n) {
    switch (n) {
      case Program p:
        PrintProgram(p);
        break;
      default:
        throw new UnsupportedNodePrintingException(n);
    }
  }

  private void PrintProgram(Program p) {
    PrintModule(p.ProgramModule);
  }

  private void PrintType(Type t) {
    Write(t.BaseName);
    if (t.HasTypeArgs()) {
      Write("<");
      ResetSeparator();
      foreach (var a in t.GetTypeArgs()) {
        WriteSeparator();
        PrintType(a);
      }
      Write(">");
    }
  }

}