using Dafny = Microsoft.Dafny;

namespace AST_new.Translation;

public partial class DafnyASTTranslator {
  private Declaration TranslateDeclRef(Dafny.Declaration dd) {
    if (TranslatedDecls.ContainsKey(dd)) {
      return TranslatedDecls[dd];
    } else if (SkeletonDecls.ContainsKey(dd)) {
      return SkeletonDecls[dd];
    }
    return CreateDeclRef(dd);
  }

  private Declaration CreateDeclRef(Dafny.Declaration dd) {
    return dd switch {
      Dafny.ModuleDecl m => CreateModuleDeclSkeleton(m),
      Dafny.ClassDecl c => CreateClassDeclSkeleton(c),
      Dafny.Method m => CreateMethodDeclSkeleton(m),
      Dafny.Function f => CreateFunctionDeclSkeleton(f),
      Dafny.DatatypeDecl d => CreateDatatypeDeclSkeleton(d),
      _ => TranslateDeclaration(dd),
    };
  }

  private Declaration TranslateDeclaration(Dafny.Declaration d) {
    return d switch {
      Dafny.TopLevelDecl tld => TranslateTopLevelDecl(tld),
      Dafny.MemberDecl md => TranslateMemberDecl(md),
      Dafny.DatatypeCtor dc => TranslateDatatypeConstructor(dc),
      _ => throw new UnsupportedTranslationException(d),
    };
  }

  private TopLevelDecl TranslateTopLevelDecl(Dafny.TopLevelDecl tld) {
    return tld switch {
      Dafny.ModuleDecl m => TranslateModuleDecl(m),
      Dafny.ClassDecl c => TranslateClassDecl(c),
      Dafny.TypeParameter tp => TranslateTypeParameter(tp),
      Dafny.SubsetTypeDecl std => TranslateSubsetTypeDecl(std),
      Dafny.TypeSynonymDecl tsd => TranslateTypeSynonymDecl(tsd),
      Dafny.DatatypeDecl dtd => TranslateDatatypeDecl(dtd),
      _ => throw new UnsupportedTranslationException(tld),
    };
  }

  private MemberDecl TranslateMemberDecl(Dafny.MemberDecl md) {
    return md switch {
      Dafny.Method m => TranslateMethod(m),
      Dafny.Function f => TranslateFunction(f),
      Dafny.Field f => f switch {
        Dafny.DatatypeDestructor d => TranslateDatatypeDestructor(d),
        Dafny.DatatypeDiscriminator d => TranslateDatatypeDiscriminator(d),
        _ => TranslateFieldDecl(f),
      },
      _ => throw new UnsupportedTranslationException(md),
    };
  }

  private ModuleDecl CreateModuleDeclSkeleton(Dafny.ModuleDecl d) {
    var s = ModuleDecl.Skeleton();
    MarkDeclSkeleton(d, s);
    return s;
  }
  private ModuleDecl TranslateModuleDecl(Dafny.ModuleDecl dm) {
    if (HasTranslatedDecl(dm)) {
      return (ModuleDecl)GetTranslatedDecl(dm);
    }
    var m = HasSkeletonDecl(dm) ?
      (ModuleDecl)GetSkeletonDecl(dm) : CreateModuleDeclSkeleton(dm);
    if (dm is Dafny.LiteralModuleDecl lm) {
      m.AddDecls(lm.ModuleDef.TopLevelDecls.Select(TranslateTopLevelDecl));
    } else {
      throw new UnsupportedTranslationException(dm);
    }
    MarkDeclTranslated(dm, m);
    return m;
  }

  private ClassDecl CreateClassDeclSkeleton(Dafny.ClassDecl d) {
    ClassDecl s = d switch {
      Dafny.DefaultClassDecl => DefaultClassDecl.Skeleton(),
      // TODO: Add to a built-ins list?
      Dafny.ArrayClassDecl a => ArrayClassDecl.Skeleton(a.Dims),
      _ => ClassDecl.Skeleton(d.Name),
    };
    MarkDeclSkeleton(d, s);
    return s;
  }
  private ClassDecl TranslateClassDecl(Dafny.ClassDecl dc) {
    if (HasTranslatedDecl(dc)) {
      return (ClassDecl)GetTranslatedDecl(dc);
    }
    var c = HasSkeletonDecl(dc) ?
      (ClassDecl)GetSkeletonDecl(dc) : CreateClassDeclSkeleton(dc);
    c.AddMembers(dc.Members.Select(TranslateMemberDecl));
    MarkDeclTranslated(dc, c);
    return c;
  }

  private TypeParameter TranslateTypeParameter(Dafny.TypeParameter dtp) {
    if (HasTranslatedDecl(dtp)) {
      return (TypeParameter)GetTranslatedDecl(dtp);
    }
    var tp = new TypeParameter(dtp.Name);
    MarkDeclTranslated(dtp, tp);
    return tp;
  }

  private TypeSynonymDecl TranslateTypeSynonymDecl(Dafny.TypeSynonymDecl dtsd) {
    if (HasTranslatedDecl(dtsd)) {
      return (TypeSynonymDecl)GetTranslatedDecl(dtsd);
    }
    var tsd = new TypeSynonymDecl(name: dtsd.Name,
      baseType: TranslateType(dtsd.Rhs),
      typeParams: dtsd.TypeArgs.Select(TranslateTypeParameter));
    MarkDeclTranslated(dtsd, tsd);
    return tsd;
  }

  private SubsetTypeDecl TranslateSubsetTypeDecl(Dafny.SubsetTypeDecl dstd) {
    if (HasTranslatedDecl(dstd)) {
      return (SubsetTypeDecl)GetTranslatedDecl(dstd);
    }
    var std = new SubsetTypeDecl(
      name: dstd.Name, baseIdent: TranslateBoundVar(dstd.Var),
      constraint: TranslateExpression(dstd.Constraint),
      typeParams: dstd.TypeArgs.Select(TranslateTypeParameter));
    MarkDeclTranslated(dstd, std);
    return std;
  }

  private MethodDecl CreateMethodDeclSkeleton(Dafny.Method d) {
    var enclosingDecl
        = (TopLevelDecl)TranslateDeclRef(d.EnclosingClass);
    var s = d switch {
      Dafny.Constructor => ConstructorDecl.Skeleton(enclosingDecl, d.Name),
      _ => MethodDecl.Skeleton(enclosingDecl, d.Name),
    };
    MarkDeclSkeleton(d, s);
    return s;
  }
  private MethodDecl TranslateMethod(Dafny.Method dm) {
    if (HasTranslatedDecl(dm)) {
      return (MethodDecl)GetTranslatedDecl(dm);
    }
    var m = HasSkeletonDecl(dm) ?
      (MethodDecl)GetSkeletonDecl(dm) : CreateMethodDeclSkeleton(dm);
    m.Body = dm.Body == null ? null : TranslateBlockStmt(dm.Body);
    m.Ins.AddRange(dm.Ins.Select(TranslateFormal));
    if (dm.Outs != null) {
      m.Outs.AddRange(dm.Outs.Select(TranslateFormal));
    }
    m.Precondition
      = TranslateSpecification(Specification.Type.Precondition, dm.Req);
    m.Postcondition
      = TranslateSpecification(Specification.Type.Precondition, dm.Ens);
    m.Modifies
      = TranslateSpecification(Specification.Type.Precondition, dm.Mod);
    m.Decreases
      = TranslateSpecification(Specification.Type.Precondition, dm.Decreases);
    MarkDeclTranslated(dm, m);
    return m;
  }

  private FunctionDecl CreateFunctionDeclSkeleton(Dafny.Function d) {
    var enclosingDecl
        = (TopLevelDecl)TranslateDeclRef(d.EnclosingClass);
    var s = FunctionDecl.Skeleton(
      enclosingDecl, d.Name, TranslateType(d.ResultType));
    MarkDeclSkeleton(d, s);
    return s;
  }
  private FunctionDecl TranslateFunction(Dafny.Function df) {
    if (HasTranslatedDecl(df)) {
      return (FunctionDecl)GetTranslatedDecl(df);
    }
    var f = HasSkeletonDecl(df) ?
      (FunctionDecl)GetSkeletonDecl(df) : CreateFunctionDeclSkeleton(df);
    f.Body = df.Body == null ? null : TranslateExpression(df.Body);
    f.Ins.AddRange(df.Formals.Select(TranslateFormal));
    f.Result = df.Result == null ? null : TranslateFormal(df.Result);
    f.Precondition
      = TranslateSpecification(Specification.Type.Precondition, df.Req);
    f.Postcondition
      = TranslateSpecification(Specification.Type.Precondition, df.Ens);
    f.Reads
      = TranslateSpecification(Specification.Type.Precondition, df.Reads);
    f.Decreases
      = TranslateSpecification(Specification.Type.Precondition, df.Decreases);
    MarkDeclTranslated(df, f);
    return f;
  }

  private DatatypeDecl CreateDatatypeDeclSkeleton(Dafny.DatatypeDecl d) {
    var s = DatatypeDecl.Skeleton(d.Name,
      typeParams: d.TypeArgs.Select(TranslateTypeParameter));
    MarkDeclSkeleton(d, s);
    return s;
  }
  private void
  PopulateDatatypeConstructors(Dafny.DatatypeDecl dd, DatatypeDecl d) {
    foreach (var c in dd.Ctors) {
      d.AddConstructor(TranslateDatatypeConstructor(c));
    }
  }
  private DatatypeDecl
  TranslateDatatypeDecl(Dafny.DatatypeDecl dd) {
    if (HasTranslatedDecl(dd)) {
      return (DatatypeDecl)GetTranslatedDecl(dd);
    }
    var d = HasSkeletonDecl(dd) ?
      (DatatypeDecl)GetSkeletonDecl(dd) : CreateDatatypeDeclSkeleton(dd);
    PopulateDatatypeConstructors(dd, d);
    foreach (var m in dd.Members) {
      d.AddMember(TranslateMemberDecl(m));
    }
    MarkDeclTranslated(dd, d);
    return d;
  }

  private DatatypeConstructorDecl
  TranslateDatatypeConstructor(Dafny.DatatypeCtor ddc) {
    if (HasTranslatedDecl(ddc)) {
      return (DatatypeConstructorDecl)GetTranslatedDecl(ddc);
    }
    var enclosingDecl = (DatatypeDecl)TranslateDeclRef(ddc.EnclosingDatatype);
    var dc = new DatatypeConstructorDecl(enclosingDecl,
      ddc.Name, ddc.Formals.Select(TranslateFormal));
    MarkDeclTranslated(ddc, dc);
    return dc;
  }

  private DatatypeDestructorDecl
  TranslateDatatypeDestructor(Dafny.DatatypeDestructor ddd) {
    if (HasTranslatedDecl(ddd)) {
      return (DatatypeDestructorDecl)GetTranslatedDecl(ddd);
    }
    // Don't try to create a destructor directly, it should be auto generated
    // when adding the corresponding constructor to the base datatype.
    var enclosingDecl = (DatatypeDecl)TranslateDeclRef(ddd.EnclosingClass);
    if (!HasTranslatedDecl(ddd.EnclosingClass)) {
      PopulateDatatypeConstructors(
        (Dafny.DatatypeDecl)ddd.EnclosingClass, enclosingDecl);
    }
    var dd = enclosingDecl.GetDestructor(ddd.Name);
    MarkDeclTranslated(ddd, dd);
    return dd;
  }

  private DatatypeDiscriminatorDecl
  TranslateDatatypeDiscriminator(Dafny.DatatypeDiscriminator ddd) {
    if (HasTranslatedDecl(ddd)) {
      return (DatatypeDiscriminatorDecl)GetTranslatedDecl(ddd);
    }
    // Don't try to create a discriminator directly, it should be auto generated
    // when adding the corresponding constructor to the base datatype.
    var enclosingDecl = (DatatypeDecl)TranslateDeclRef(ddd.EnclosingClass);
    if (!HasTranslatedDecl(ddd.EnclosingClass)) {
      PopulateDatatypeConstructors(
        (Dafny.DatatypeDecl)ddd.EnclosingClass, enclosingDecl);
    }
    var dd = enclosingDecl.GetDiscriminator(ddd.Name);
    MarkDeclTranslated(ddd, dd);
    return dd;
  }

  private FieldDecl TranslateFieldDecl(Dafny.Field df) {
    if (HasTranslatedDecl(df)) {
      return (FieldDecl)GetTranslatedDecl(df);
    }
    var enclosingDecl = (TopLevelDecl)TranslateDeclRef(df.EnclosingClass);
    var f = new FieldDecl(enclosingDecl, df.Name, TranslateType(df.Type));
    MarkDeclTranslated(df, f);
    return f;
  }

}
