namespace AST;

public class Statement
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
      Dafny.WhileStmt whileStmt
        => WhileStmt.FromDafny(whileStmt),
      _ => throw new NotImplementedException(),
    };
  }
}