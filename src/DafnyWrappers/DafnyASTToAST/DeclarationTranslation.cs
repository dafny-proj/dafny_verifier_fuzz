using Dafny = Microsoft.Dafny;

namespace AST_new.Translation;

public partial class DafnyASTTranslator {
  public TopLevelDecl TranslateTopLevelDecl(Dafny.TopLevelDecl tld) {
    return tld switch {
      Dafny.ModuleDecl m => TranslateModule(m),
      Dafny.ClassDecl c => TranslateClass(c),
      _ => throw new UnsupportedTranslationException(tld),
    };
  }

  public ModuleDecl TranslateModule(Dafny.ModuleDecl m) {
    if (m is Dafny.LiteralModuleDecl lmd) {
      var tm = (ModuleDecl)GetTranslatedDeclOrCreateSkeleton(lmd);
      tm.AddDecls(lmd.ModuleDef.TopLevelDecls.Select(TranslateTopLevelDecl));
      return tm;
    }
    throw new UnsupportedTranslationException(m);
  }

  public ClassDecl TranslateClass(Dafny.ClassDecl c) {
    var tc = (ClassDecl)GetTranslatedDeclOrCreateSkeleton(c);
    tc.AddMembers(c.Members.Select(TranslateMemberDecl));
    return tc;
  }

  public MemberDecl TranslateMemberDecl(Dafny.MemberDecl md) {
    return md switch {
      Dafny.Method mtd => TranslateMethod(mtd),
      _ => throw new UnsupportedTranslationException(md),
    };
  }

  public MethodDecl TranslateMethod(Dafny.Method m) {
    var tm = (MethodDecl)GetTranslatedDeclOrCreateSkeleton(m);
    tm.Body = m.Body == null ? null : TranslateBlockStmt(m.Body);
    tm.Ins.AddRange(m.Ins.Select(TranslateFormal));
    if (m.Outs != null) {
      tm.Outs.AddRange(m.Outs.Select(TranslateFormal));
    }
    tm.Precondition
      = TranslateSpecification(Specification.Type.Precondition, m.Req);
    tm.Postcondition
      = TranslateSpecification(Specification.Type.Precondition, m.Ens);
    tm.Modifies
      = TranslateSpecification(Specification.Type.Precondition, m.Mod);
    tm.Decreases
      = TranslateSpecification(Specification.Type.Precondition, m.Decreases);
    return tm;
  }

  public Specification? TranslateSpecification<T>(Specification.Type st,
  Dafny.Specification<T> es) where T : Dafny.Node {
    return TranslateSpecification<T>(st, es.Expressions);
  }

  public Specification? TranslateSpecification<T>(Specification.Type st,
  List<T> es) where T : Dafny.Node {
    if (es.Count > 0) {
      return new Specification(st, es.Select(TranslateExpression));
    }
    return null;
  }

}
