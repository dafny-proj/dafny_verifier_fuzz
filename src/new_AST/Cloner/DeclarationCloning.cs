namespace AST_new.Cloner;

public partial class ASTCloner {
  private Declaration CloneDeclRef(Declaration d) {
    if (!DeclarationsToClone.ContainsKey(d)) {
      return d;
    }
    if (ClonedDeclarations.ContainsKey(d)) {
      return ClonedDeclarations[d];
    }
    if (DeclarationsToClone[d] != null) {
      return DeclarationsToClone[d]!;
    }
    return CreateDeclCloneRef(d);
  }

  private Declaration CreateDeclCloneRef(Declaration d) {
    // Create a skeleton clone of the declaration that can be referenced.
    var clone = d switch {
      ModuleDecl md => CreateModuleDeclSkeleton(md),
      ClassDecl cd => CreateClassDeclSkeleton(cd),
      DatatypeDecl dd => CreateDatatypeDeclSkeleton(dd),
      MethodDecl md => CreateMethodDeclSkeleton(md),
      FunctionDecl fd => CreateFunctionDeclSkeleton(fd),
      _ => CloneDeclaration(d),
    };
    return clone;
  }

  private Declaration CloneDeclaration(Declaration d) {
    return d switch {
      TopLevelDecl td => CloneTopLevelDecl(td),
      MemberDecl md => CloneMemberDecl(md),
      _ => throw new UnsupportedNodeCloningException(d),
    };
  }

  private TopLevelDecl CloneTopLevelDecl(TopLevelDecl d) {
    return d switch {
      ModuleDecl md => CloneModuleDecl(md),
      ClassDecl cd => CloneClassDecl(cd),
      TypeParameterDecl td => CloneTypeParameterDecl(td),
      TypeSynonymDecl td => CloneTypeSynonymDecl(td),
      SubsetTypeDecl sd => CloneSubsetTypeDecl(sd),
      DatatypeDecl dd => CloneDatatypeDecl(dd),
      _ => throw new UnsupportedNodeCloningException(d),
    };
  }

  private MemberDecl CloneMemberDecl(MemberDecl d) {
    return d switch {
      MethodDecl md => CloneMethodDecl(md),
      FunctionDecl fd => CloneFunctionDecl(fd),
      FieldDecl fd => fd switch {
        DatatypeDestructorDecl dd => CloneDatatypeDestructorDecl(dd),
        DatatypeDiscriminatorDecl dd => CloneDatatypeDiscriminatorDecl(dd),
        _ => CloneFieldDecl(fd),
      },
      DatatypeConstructorDecl cd => CloneDatatypeConstructorDecl(cd),
      _ => throw new UnsupportedNodeCloningException(d),
    };
  }

  private ModuleDecl CreateModuleDeclSkeleton(ModuleDecl d) {
    var s = ModuleDecl.Skeleton();
    MarkDeclCloneSkeleton(d, s);
    return s;
  }
  private ModuleDecl CloneModuleDecl(ModuleDecl d) {
    if (HasClonedDecl(d)) {
      return (ModuleDecl)GetClonedDecl(d);
    }
    var c = HasSkeletonDeclClone(d) ?
      (ModuleDecl)GetSkeletonDeclClone(d) : CreateModuleDeclSkeleton(d);
    c.AddDecls(d.Decls.Select(CloneTopLevelDecl));
    MarkDeclCloned(d, c);
    return c;
  }

  private ClassDecl CreateClassDeclSkeleton(ClassDecl d) {
    var s = d switch {
      DefaultClassDecl => DefaultClassDecl.Skeleton(),
      ArrayClassDecl a => ArrayClassDecl.Skeleton(a.Dimensions),
      _ => ClassDecl.Skeleton(d.Name),
    };
    MarkDeclCloneSkeleton(d, s);
    return s;
  }
  private ClassDecl CloneClassDecl(ClassDecl d) {
    if (HasClonedDecl(d)) {
      return (ClassDecl)GetClonedDecl(d);
    }
    var c = HasSkeletonDeclClone(d) ?
      (ClassDecl)GetSkeletonDeclClone(d) : CreateClassDeclSkeleton(d);
    c.AddMembers(d.Members.Select(CloneMemberDecl));
    MarkDeclCloned(d, c);
    return c;
  }

  private TypeParameterDecl CloneTypeParameterDecl(TypeParameterDecl d) {
    if (HasClonedDecl(d)) {
      return (TypeParameterDecl)GetClonedDecl(d);
    }
    var c = new TypeParameterDecl(d.Name);
    MarkDeclCloned(d, c);
    return c;
  }

  private TypeSynonymDecl CloneTypeSynonymDecl(TypeSynonymDecl d) {
    if (HasClonedDecl(d)) {
      return (TypeSynonymDecl)GetClonedDecl(d);
    }
    var c = new TypeSynonymDecl(name: d.Name,
      baseType: CloneType(d.BaseType),
      typeParams: d.TypeParams.Select(CloneTypeParameterDecl));
    MarkDeclCloned(d, c);
    return c;
  }

  private SubsetTypeDecl CloneSubsetTypeDecl(SubsetTypeDecl d) {
    if (HasClonedDecl(d)) {
      return (SubsetTypeDecl)GetClonedDecl(d);
    }
    var c = new SubsetTypeDecl(name: d.Name,
      baseIdent: CloneBoundVar(d.BaseIdent),
      constraint: CloneExpression(d.Constraint),
      typeParams: d.TypeParams.Select(CloneTypeParameterDecl));
    MarkDeclCloned(d, c);
    return c;
  }

  private DatatypeDecl CreateDatatypeDeclSkeleton(DatatypeDecl d) {
    var s = DatatypeDecl.Skeleton(name: d.Name,
      typeParams: d.TypeParams.Select(CloneTypeParameterDecl));
    MarkDeclCloneSkeleton(d, s);
    return s;
  }
  private void
  PopulateDatatypeConstructors(DatatypeDecl d, DatatypeDecl c) {
    c.AddConstructors(d.Constructors.Select(CloneDatatypeConstructorDecl));
  }
  private DatatypeDecl CloneDatatypeDecl(DatatypeDecl d) {
    if (HasClonedDecl(d)) {
      return (DatatypeDecl)GetClonedDecl(d);
    }
    var c = HasSkeletonDeclClone(d) ?
      (DatatypeDecl)GetSkeletonDeclClone(d) : CreateDatatypeDeclSkeleton(d);
    PopulateDatatypeConstructors(d, c);
    c.AddMembers(d.Members.Select(CloneMemberDecl));
    MarkDeclCloned(d, c);
    return c;
  }

  private DatatypeConstructorDecl
  CloneDatatypeConstructorDecl(DatatypeConstructorDecl d) {
    if (HasClonedDecl(d)) {
      return (DatatypeConstructorDecl)GetClonedDecl(d);
    }
    var enclosingDecl = (DatatypeDecl)CloneDeclRef(d.EnclosingDecl);
    var c = new DatatypeConstructorDecl(enclosingDecl,
      name: d.Name, parameters: d.Parameters.Select(CloneFormal));
    MarkDeclCloned(d, c);
    return c;
  }

  private DatatypeDestructorDecl
  CloneDatatypeDestructorDecl(DatatypeDestructorDecl d) {
    if (HasClonedDecl(d)) {
      return (DatatypeDestructorDecl)GetClonedDecl(d);
    }
    // Don't try to create a destructor directly, it should be auto generated
    // when adding the corresponding constructor to the base datatype.
    var enclosingDecl = (DatatypeDecl)CloneDeclRef(d.EnclosingDecl);
    if (enclosingDecl.Constructors.Count <= 0) {
      PopulateDatatypeConstructors(
        (DatatypeDecl)d.EnclosingDecl, enclosingDecl);
    }
    var c = enclosingDecl.GetDestructor(d.Name);
    MarkDeclCloned(d, c);
    return c;
  }

  private DatatypeDiscriminatorDecl
  CloneDatatypeDiscriminatorDecl(DatatypeDiscriminatorDecl d) {
    if (HasClonedDecl(d)) {
      return (DatatypeDiscriminatorDecl)GetClonedDecl(d);
    }
    // Don't try to create a discriminator directly, it should be auto generated
    // when adding the corresponding constructor to the base datatype.
    var enclosingDecl = (DatatypeDecl)CloneDeclRef(d.EnclosingDecl);
    if (enclosingDecl.Constructors.Count <= 0) {
      PopulateDatatypeConstructors(
        (DatatypeDecl)d.EnclosingDecl, enclosingDecl);
    }
    var c = enclosingDecl.GetDiscriminator(d.Name);
    MarkDeclCloned(d, c);
    return c;
  }

  private FieldDecl CloneFieldDecl(FieldDecl d) {
    if (HasClonedDecl(d)) {
      return (FieldDecl)GetClonedDecl(d);
    }
    var enclosingDecl = (TopLevelDecl)CloneDeclRef(d.EnclosingDecl);
    var c = new FieldDecl(enclosingDecl, name: d.Name, type: CloneType(d.Type));
    MarkDeclCloned(d, c);
    return c;
  }

  private MethodDecl CreateMethodDeclSkeleton(MethodDecl d) {
    var enclosingDecl = (TopLevelDecl)CloneDeclRef(d.EnclosingDecl);
    var s = d switch {
      ConstructorDecl => ConstructorDecl.Skeleton(enclosingDecl, d.Name),
      _ => MethodDecl.Skeleton(enclosingDecl, d.Name),
    };
    MarkDeclCloneSkeleton(d, s);
    return s;
  }
  private MethodDecl CloneMethodDecl(MethodDecl d) {
    if (HasClonedDecl(d)) {
      return (MethodDecl)GetClonedDecl(d);
    }
    var c = HasSkeletonDeclClone(d) ?
      (MethodDecl)GetSkeletonDeclClone(d) : CreateMethodDeclSkeleton(d);
    c.TypeParams.AddRange(d.TypeParams.Select(CloneTypeParameterDecl));
    c.Body = d.Body == null ? null : CloneBlockStmt(d.Body);
    c.Ins.AddRange(d.Ins.Select(CloneFormal));
    if (d.Outs != null) {
      c.Outs.AddRange(d.Outs.Select(CloneFormal));
    }
    c.Precondition = CloneSpecification(d.Precondition);
    c.Postcondition = CloneSpecification(d.Postcondition);
    c.Modifies = CloneSpecification(d.Modifies);
    c.Decreases = CloneSpecification(d.Decreases);
    MarkDeclCloned(d, c);
    return c;
  }

  private FunctionDecl CreateFunctionDeclSkeleton(FunctionDecl d) {
    var enclosingDecl = (TopLevelDecl)CloneDeclRef(d.EnclosingDecl);
    var s = FunctionDecl.Skeleton(
      enclosingDecl, d.Name, CloneType(d.ResultType));
    MarkDeclCloneSkeleton(d, s);
    return s;
  }
  private FunctionDecl CloneFunctionDecl(FunctionDecl d) {
    if (HasClonedDecl(d)) {
      return (FunctionDecl)GetClonedDecl(d);
    }
    var c = HasSkeletonDeclClone(d) ?
      (FunctionDecl)GetSkeletonDeclClone(d) : CreateFunctionDeclSkeleton(d);
    c.Body = d.Body == null ? null : CloneExpression(d.Body);
    c.Ins.AddRange(d.Ins.Select(CloneFormal));
    c.Result = d.Result == null ? null : CloneFormal(d.Result);
    c.Precondition = CloneSpecification(d.Precondition);
    c.Postcondition = CloneSpecification(d.Postcondition);
    c.Reads = CloneSpecification(d.Reads);
    c.Decreases = CloneSpecification(d.Decreases);
    MarkDeclCloned(d, c);
    return c;
  }

}
