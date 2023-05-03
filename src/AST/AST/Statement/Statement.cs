namespace AST;

public abstract class Statement
: Node, ConstructableFromDafny<Dafny.Statement, Statement> {
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
      _ => throw new NotImplementedException($"Unhandled translation from Dafny for `{dafnyNode.GetType()}`"),
    };
  }

  public override Statement Clone() {
    throw new NotSupportedException($"Cloning unhandled for {this.GetType()}");
  }
}