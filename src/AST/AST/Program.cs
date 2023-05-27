namespace AST;

public partial class Program : Node {
  public ModuleDecl ProgramModule { get; }

  public Program(ModuleDecl programModule) {
    ProgramModule = programModule;
  }

  public override IEnumerable<Node> Children => new[] { ProgramModule };
}