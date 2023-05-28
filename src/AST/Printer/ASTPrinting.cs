namespace AST.Printer;

public partial class ASTPrinter {
  private TextWriter _wr { get; }
  private void Write(string s) => _wr.Write(s);
  private void WriteLine(string? s = null) => _wr.WriteLine(s);

  private int _indent = 0;
  private void IncIndent(int n = 1) => _indent += n;
  private void DecIndent(int n = 1) => _indent -= n;
  private void WriteIndent() => Write(new String(' ', _indent * 2));

  public ASTPrinter(TextWriter wr) {
    _wr = wr;
  }

  public static void PrintNode(Node n, TextWriter wr) {
    var pr = new ASTPrinter(wr);
    pr.PrintNode(n);
  }

  public static string PrintNodeToString(Node n) {
    var wr = new StringWriter();
    PrintNode(n, wr);
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

  private delegate void PrintFunction<N>(N n);
  private void PrintList<N>(IEnumerable<N> ns, PrintFunction<N> pr,
  string start = "", string sep = ", ", string end = "") {
    Write(start);
    var _sep = "";
    foreach (var n in ns) {
      Write(_sep);
      _sep = sep;
      pr(n);
    }
    Write(end);
  }
  private void PrintExpressions(IEnumerable<Expression> es) {
    PrintList<Expression>(es, PrintExpression);
  }
  private void PrintExpressionPairs(IEnumerable<ExpressionPair> eps) {
    PrintList<ExpressionPair>(eps, PrintExpressionPair);
  }
  private void PrintLocalVars(IEnumerable<LocalVar> lvs) {
    PrintList<LocalVar>(lvs, PrintLocalVar);
  }
  private void PrintAssignmentRhss(IEnumerable<AssignmentRhs> ars) {
    PrintList<AssignmentRhs>(ars, PrintAssignmentRhs);
  }
  private void PrintTypeParameters(IEnumerable<TypeParameterDecl> tps) {
    if (tps.Count() > 0) {
      PrintList<TypeParameterDecl>(
        tps, PrintTypeParameterDecl, start: "<", end: ">");
    }
  }
  private void PrintFormals(IEnumerable<Formal> fs) {
    PrintList<Formal>(fs, PrintFormal, start: "(", end: ")");
  }
  private void PrintMembers(IEnumerable<MemberDecl> ds) {
    var fields = ds.Where(m => m is FieldDecl);
    var remaining = ds.Where(m => m is not FieldDecl);
    PrintList<MemberDecl>(fields, PrintMemberDecl, sep: "");
    PrintList<MemberDecl>(remaining, PrintMemberDecl, sep: "\n");
  }
  private void PrintTypes(IEnumerable<Type> ts) {
    PrintList<Type>(ts, PrintType, start: "<", end: ">");
  }

  private void PrintProgram(Program p) {
    PrintModuleDecl(p.ProgramModule);
  }

  private void PrintType(Type t) {
    Write(t.BaseName);
    if (t.HasTypeArgs()) {
      PrintTypes(t.GetTypeArgs());
    }
  }

}