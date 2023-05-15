namespace AST;

public class TopLevelDecl
: Declaration, ConstructableFromDafny<Dafny.TopLevelDecl, TopLevelDecl> {
  public static TopLevelDecl FromDafny(Dafny.TopLevelDecl dafnyNode) {
    return dafnyNode switch {
      Dafny.ModuleDecl moduleDecl
        => ModuleDecl.FromDafny(moduleDecl),
      Dafny.ClassDecl classDecl
        => ClassDecl.FromDafny(classDecl),
      Dafny.IndDatatypeDecl indDatatypeDecl
        => InductiveDatatypeDecl.FromDafny(indDatatypeDecl),
      Dafny.TypeParameter typeParam
        => TypeParameter.FromDafny(typeParam),
      _ => throw new NotImplementedException(
          $"Unhandled translation from Dafny for `{dafnyNode.GetType()}`."
        ),
    };
  }
}

public class ModuleDecl
: TopLevelDecl, ConstructableFromDafny<Dafny.ModuleDecl, ModuleDecl> {
  public static ModuleDecl FromDafny(Dafny.ModuleDecl dafnyNode) {
    return dafnyNode switch {
      Dafny.LiteralModuleDecl litModuleDecl
        => LiteralModuleDecl.FromDafny(litModuleDecl),
      _ => throw new NotImplementedException(),
    };
  }
}

public class LiteralModuleDecl
: ModuleDecl, ConstructableFromDafny<Dafny.LiteralModuleDecl, LiteralModuleDecl> {
  public override IEnumerable<Node> Children => new[] { ModuleDef };
  public ModuleDefinition ModuleDef { get; set; }

  private LiteralModuleDecl(Dafny.LiteralModuleDecl moduleDeclDafny) {
    ModuleDef = ModuleDefinition.FromDafny(moduleDeclDafny.ModuleDef);
  }

  public static LiteralModuleDecl FromDafny(Dafny.LiteralModuleDecl dafnyNode) {
    return new LiteralModuleDecl(dafnyNode);
  }
}

public class TypeParameter
: TopLevelDecl, ConstructableFromDafny<Dafny.TypeParameter, TypeParameter> {
  // TODO: ParentType, Variance, Characteristics
  public string Name { get; }

  public TypeParameter(string name) {
    Name = name;
  }

  private TypeParameter(Dafny.TypeParameter tpd)
  : this(tpd.Name) { }

  public static TypeParameter FromDafny(Dafny.TypeParameter dafnyNode) {
    return new TypeParameter(dafnyNode);
  }
}