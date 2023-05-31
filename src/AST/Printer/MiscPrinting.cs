namespace AST.Printer;

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

  private void PrintSpecification(Specification? s, bool oneLine = false) {
    if (!Specification.HasUserDefinedSpec(s)) return;
    var st = s!.SpecificationType;
    var sts = Specification.SpecificationTypeAsString(st);
    var userDefinedExprs = s.GetUserDefinedSpec();
    switch (st) {
      case Specification.Type.Precondition:
      case Specification.Type.Postcondition:
      case Specification.Type.Invariant:
        foreach (var e in userDefinedExprs) {
          if (oneLine) {
            Write(" ");
          } else {
            WriteIndent();
          }
          Write($"{sts} ");
          PrintExpression(e);
          if (!oneLine) {
            WriteLine();
          }
        }
        break;
      case Specification.Type.ModifiesFrame:
      case Specification.Type.ReadFrame:
      case Specification.Type.Decreases:
        if (oneLine) {
          Write(" ");
        } else {
          WriteIndent();
        }
        Write($"{sts} ");
        PrintExpressions(userDefinedExprs);
        if (!oneLine) {
          WriteLine();
        }
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

  private void PrintQuantifierDomain(QuantifierDomain qd) {
    PrintBoundVars(qd.Vars);
    if (qd.Range != null) {
      Write(" | ");
      PrintExpression(qd.Range);
    }
  }

  private void PrintMatchExprCase(MatchExprCase mc) {
    Write("case ");
    PrintMatcher(mc.Key);
    Write(" => ");
    PrintExpression(mc.Value);
  }

  private void PrintMatchStmtCase(MatchStmtCase mc) {
    Write("case ");
    PrintMatcher(mc.Key);
    Write(" => ");
    if (mc.Value.Body.Count > 0) {
      PrintBlockStmt(mc.Value);
    }
  }

  private void PrintMatcher(Matcher m) {
    if (m is WildcardMatcher) {
      Write("_");
    } else if (m is ExpressionMatcher em) {
      PrintExpression(em.E);
    } else if (m is BindingMatcher bm) {
      PrintVariable(bm.Var);
    } else if (m is DestructuringMatcher dsm) {
      var isTuple = dsm.Constructor.EnclosingDecl is TupleTypeDecl;
      if (!isTuple) { Write(dsm.Constructor.Name); }
      if (dsm.ArgumentMatchers.Count > 0 || isTuple) {
        PrintList<Matcher>(dsm.ArgumentMatchers, PrintMatcher,
          start: "(", sep: ", ", end: ")");
      }
    } else if (m is DisjunctiveMatcher djm) {
      PrintList<Matcher>(djm.Matchers, PrintMatcher, sep: " | ");
    } else {
      throw new UnsupportedNodePrintingException(m);
    }
  }

}
