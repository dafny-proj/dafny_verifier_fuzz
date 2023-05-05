namespace AST;

public abstract class Statement
: Node, ConstructableFromDafny<Dafny.Statement, Statement> {

  // TODO: handle label initialisation and propagation.
  public string? Label { get; set; }

  public static Statement FromDafny(Dafny.Statement dafnyNode) {
    return dafnyNode switch {
      Dafny.BlockStmt blockStmt
        => BlockStmt.FromDafny(blockStmt),
      Dafny.ConcreteUpdateStatement concrUpdateStmt
        => ConcreteUpdateStatement.FromDafny(concrUpdateStmt),
      Dafny.IfStmt ifStmt
        => IfStmt.FromDafny(ifStmt),
      Dafny.ReturnStmt retStmt
        => ReturnStmt.FromDafny(retStmt),
      Dafny.VarDeclStmt varDeclStmt
        => VarDeclStmt.FromDafny(varDeclStmt),
      Dafny.CallStmt callStmt
        => CallStmt.FromDafny(callStmt),
      Dafny.LoopStmt loopStmt
        => LoopStmt.FromDafny(loopStmt),
      Dafny.PrintStmt printStmt
        => PrintStmt.FromDafny(printStmt),
      Dafny.BreakStmt breakStmt
        => BreakStmt.FromDafny(breakStmt),
      _ => throw new NotImplementedException($"Unhandled translation from Dafny for `{dafnyNode.GetType()}`"),
    };
  }

  public override Statement Clone() {
    throw new NotSupportedException($"Cloning unhandled for {this.GetType()}");
  }

  // TODO: handle common baseclass cloning implementation
  protected void Clone(Statement Clone) {
    Clone.Label = Label;
  }
}