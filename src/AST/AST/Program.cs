namespace AST;

public class Program : Node, ConstructableFromDafny<Dafny.Program, Program> {
  public ModuleDecl DefaultModule { get; set; }
  public ModuleDefinition DefaultModuleDef {
    get {
      return ((LiteralModuleDecl)DefaultModule).ModuleDef;
    }
  }

  private Program(Dafny.Program programDafny) {
    DefaultModule = ModuleDecl.FromDafny(programDafny.DefaultModule);
  }

  public static Program FromDafny(Dafny.Program dafnyNode) {
    return new Program(dafnyNode);
  }
}