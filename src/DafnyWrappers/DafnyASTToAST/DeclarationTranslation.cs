using Dafny = Microsoft.Dafny;

namespace AST_new.Translation;

public partial class DafnyASTTranslator {
  private Declaration CreateDeclSkeleton(Dafny.Declaration dd) {
    if (dd is Dafny.ModuleDecl) { return ModuleDecl.Skeleton(); }
    if (dd is Dafny.ClassDecl) {
      if (dd is Dafny.DefaultClassDecl) { return DefaultClassDecl.Skeleton(); }
      if (dd is Dafny.ArrayClassDecl ad) {
        // TODO: Add to a built-ins list?
        return ArrayClassDecl.Skeleton(ad.Dims);
      }
      return ClassDecl.Skeleton(dd.Name);
    }
    if (dd is Dafny.TypeSynonymDecl tsd) {
      if (tsd is Dafny.SubsetTypeDecl std) {
        return new SubsetTypeDecl(
          name: std.Name, baseIdent: TranslateBoundVar(std.Var),
          constraint: TranslateExpression(std.Constraint),
          typeParams: std.TypeArgs.Select(TranslateTypeParameter));
      }
      return new TypeSynonymDecl(name: tsd.Name,
        baseType: TranslateType(tsd.Rhs),
        typeParams: tsd.TypeArgs.Select(TranslateTypeParameter));
    }
    if (dd is Dafny.MemberDecl md) {
      var enclosingDecl
        = (TopLevelDecl)GetTranslatedDeclOrCreateSkeleton(md.EnclosingClass);
      if (dd is Dafny.Method) {
        if (dd is Dafny.Constructor) {
          return ConstructorDecl.Skeleton(enclosingDecl, dd.Name);
        }
        return MethodDecl.Skeleton(enclosingDecl, dd.Name);
      }
    }
    throw new NotImplementedException();
  }

  private TopLevelDecl TranslateTopLevelDecl(Dafny.TopLevelDecl tld) {
    return tld switch {
      Dafny.ModuleDecl m => TranslateModule(m),
      Dafny.ClassDecl c => TranslateClass(c),
      Dafny.TypeParameter tp => TranslateTypeParameter(tp),
      Dafny.SubsetTypeDecl std => TranslateSubsetTypeDecl(std),
      Dafny.TypeSynonymDecl tsd => TranslateTypeSynonymDecl(tsd),
      _ => throw new UnsupportedTranslationException(tld),
    };
  }

  private ModuleDecl TranslateModule(Dafny.ModuleDecl m) {
    if (m is Dafny.LiteralModuleDecl lmd) {
      var tm = (ModuleDecl)GetTranslatedDeclOrCreateSkeleton(lmd);
      tm.AddDecls(lmd.ModuleDef.TopLevelDecls.Select(TranslateTopLevelDecl));
      return tm;
    }
    throw new UnsupportedTranslationException(m);
  }

  private ClassDecl TranslateClass(Dafny.ClassDecl c) {
    var tc = (ClassDecl)GetTranslatedDeclOrCreateSkeleton(c);
    tc.AddMembers(c.Members.Select(TranslateMemberDecl));
    return tc;
  }

  private TypeParameter TranslateTypeParameter(Dafny.TypeParameter tp) {
    return new TypeParameter(tp.Name);
  }

  private TypeSynonymDecl TranslateTypeSynonymDecl(Dafny.TypeSynonymDecl tsd) {
    return (TypeSynonymDecl)GetTranslatedDeclOrCreateSkeleton(tsd);
  }

  private SubsetTypeDecl TranslateSubsetTypeDecl(Dafny.SubsetTypeDecl std) {
    return (SubsetTypeDecl)GetTranslatedDeclOrCreateSkeleton(std);
  }

  private MemberDecl TranslateMemberDecl(Dafny.MemberDecl md) {
    return md switch {
      Dafny.Method mtd => TranslateMethod(mtd),
      _ => throw new UnsupportedTranslationException(md),
    };
  }

  private MethodDecl TranslateMethod(Dafny.Method m) {
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

  private ConstructorDecl TranslateConstructor(Dafny.Constructor c) {
    return (ConstructorDecl)TranslateMethod(c);
  }

}
