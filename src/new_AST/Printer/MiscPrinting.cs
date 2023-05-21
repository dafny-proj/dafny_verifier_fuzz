namespace AST_new.Printer;

public partial class ASTPrinter {

  private void PrintVariable(Variable v) {
    Write(v.Name);
    if (v.HasExplicitType()) {
      Write(": ");
      PrintType(v.ExplicitType!);
    }
  }

  private void PrintLocalVar(LocalVar v) {
    PrintVariable(v);
  }

  private void PrintBoundVar(BoundVar v) {
    PrintVariable(v);
  }

  private void PrintFormal(Formal f) {
    PrintVariable(f);
    if (f.HasDefaultValue()) {
      Write(" := ");
      PrintExpression(f.DefaultValue!);
    }
  }

  private void PrintSpecification(Specification? s) {
    if (!Specification.HasUserDefinedSpec(s)) return;
    var st = s!.SpecificationType;
    var sts = Specification.SpecificationTypeAsString(st);
    var userDefinedExprs = s.GetUserDefinedSpec();
    switch (st) {
      case Specification.Type.Precondition:
      case Specification.Type.Postcondition:
      case Specification.Type.Invariant:
        foreach (var e in userDefinedExprs) {
          WriteIndent();
          Write($"{sts} ");
          PrintExpression(e);
          WriteLine();
        }
        break;
      case Specification.Type.ModifiesFrame:
      case Specification.Type.ReadFrame:
      case Specification.Type.Decreases:
        WriteIndent();
        Write($"{sts} ");
        PrintExpressions(userDefinedExprs);
        WriteLine();
        break;
      default:
        throw new UnsupportedNodePrintingException(s);
    }
  }

  private void PrintExpressionPair(ExpressionPair ep) {
    PrintExpression(ep.Key);
    Write(" := ");
    PrintExpression(ep.Value);
  }

}
