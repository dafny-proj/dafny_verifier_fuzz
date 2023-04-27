namespace AST;

public class TopLevelDecl
: Declaration, ConstructableFromDafny<Dafny.TopLevelDecl, TopLevelDecl> {
  public static TopLevelDecl FromDafny(Dafny.TopLevelDecl dafnyNode) {
    return dafnyNode switch {
      Dafny.ModuleDecl moduleDecl
        => ModuleDecl.FromDafny(moduleDecl),
      Dafny.ClassDecl classDecl
        => ClassDecl.FromDafny(classDecl),
      _ => throw new NotImplementedException(),
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

public class ClassDecl
: TopLevelDecl, ConstructableFromDafny<Dafny.ClassDecl, ClassDecl> {
  public override IEnumerable<Node> Children => Members;
  public List<MemberDecl> Members = new List<MemberDecl>();
  public readonly bool IsDefaultClass = false;

  private ClassDecl(Dafny.ClassDecl classDeclDafny) {
    Members.AddRange(classDeclDafny.Members.Select(MemberDecl.FromDafny));
    IsDefaultClass = classDeclDafny.IsDefaultClass;
  }

  public static ClassDecl FromDafny(Dafny.ClassDecl dafnyNode) {
    return new ClassDecl(dafnyNode);
  }
}