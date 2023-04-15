namespace AST;

public class Statement
: Node, ConstructableFromDafny<Dafny.Statement, Statement> {
  public static Statement FromDafny(Dafny.Statement dafnyNode) {
    return dafnyNode switch {
      Dafny.BlockStmt blockStmt
        => BlockStmt.FromDafny(blockStmt),
      Dafny.UpdateStmt updateStmt
        => UpdateStmt.FromDafny(updateStmt),
      Dafny.IfStmt ifStmt
        => IfStmt.FromDafny(ifStmt),
      _ => throw new NotImplementedException(),
    };
  }
}

public class BlockStmt
: Statement, ConstructableFromDafny<Dafny.BlockStmt, BlockStmt> {
  public List<Statement> Body = new List<Statement>();
  private BlockStmt(Dafny.BlockStmt blockStmtDafny) {
    Body.AddRange(blockStmtDafny.Body.Select(Statement.FromDafny));
  }
  public static BlockStmt FromDafny(Dafny.BlockStmt dafnyNode) {
    return new BlockStmt(dafnyNode);
  }
}

public class UpdateStmt
: Statement, ConstructableFromDafny<Dafny.UpdateStmt, UpdateStmt> {
  public List<Expression> Lhss = new List<Expression>();
  public List<AssignmentRhs> Rhss = new List<AssignmentRhs>();

  private UpdateStmt(Dafny.UpdateStmt updateStmtDafny) {
    Lhss.AddRange(updateStmtDafny.Lhss.Select(Expression.FromDafny));
    Rhss.AddRange(updateStmtDafny.Rhss.Select(AssignmentRhs.FromDafny));
  }
  public static UpdateStmt FromDafny(Dafny.UpdateStmt dafnyNode) {
    return new UpdateStmt(dafnyNode);
  }
}

public class IfStmt
: Statement, ConstructableFromDafny<Dafny.IfStmt, IfStmt> {

  public Expression? Guard { get; set; }
  public BlockStmt Thn { get; set; }
  public Statement? Els { get; set; }

  private IfStmt(Dafny.IfStmt ifStmtDafny) {
    Guard = ifStmtDafny.Guard == null ? null : Expression.FromDafny(ifStmtDafny.Guard);
    Thn = BlockStmt.FromDafny(ifStmtDafny.Thn);
    Els = ifStmtDafny.Els == null ? null : Statement.FromDafny(ifStmtDafny.Els);
  }

  public static IfStmt FromDafny(Dafny.IfStmt dafnyNode) {
    return new IfStmt(dafnyNode);
  }
}