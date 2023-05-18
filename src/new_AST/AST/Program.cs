namespace AST_new;

public partial class Program : Node {
  public ModuleDecl ProgramModule { get; }

  public Program(ModuleDecl programModule) {
    ProgramModule = programModule;
  }
}