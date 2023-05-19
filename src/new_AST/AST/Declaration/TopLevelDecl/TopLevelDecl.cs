namespace AST_new;

public abstract partial class TopLevelDecl : Declaration { }
public partial class ModuleDecl : TopLevelDecl { }
public partial class ClassDecl : TopLevelDecl { }
public partial class TypeParameter : TopLevelDecl { }
public partial class TypeSynonymDecl : TopLevelDecl { }
public partial class SubsetTypeDecl : TopLevelDecl { }

public partial class TypeParameter : TopLevelDecl {
  public override string Name { get; protected set; }
  public TypeParameter(string name) {
    Name = name;
  }
}
