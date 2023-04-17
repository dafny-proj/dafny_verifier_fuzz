using System.Diagnostics.Contracts;

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

public abstract class ConcreteUpdateStatement
: Statement, ConstructableFromDafny<Dafny.ConcreteUpdateStatement, ConcreteUpdateStatement> {
  public List<Expression> Lhss = new List<Expression>();

  protected ConcreteUpdateStatement(Dafny.ConcreteUpdateStatement cuStmtDafny) {
    Lhss.AddRange(cuStmtDafny.Lhss.Select(Expression.FromDafny));
  }

  public static ConcreteUpdateStatement FromDafny(Dafny.ConcreteUpdateStatement cuStmtDafny) {
    return cuStmtDafny switch {
      Dafny.UpdateStmt us => UpdateStmt.FromDafny(us),
      _ => throw new NotImplementedException(),
    };
  }
}

public class UpdateStmt
: ConcreteUpdateStatement, ConstructableFromDafny<Dafny.UpdateStmt, UpdateStmt> {
  public List<AssignmentRhs> Rhss = new List<AssignmentRhs>();

  private UpdateStmt(Dafny.UpdateStmt updateStmtDafny) : base(updateStmtDafny) {
    Rhss.AddRange(updateStmtDafny.Rhss.Select(AssignmentRhs.FromDafny));
    // TODO: should this somehow use updateStmtDafny.ResolvedStatements?
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

public class ReturnStmt
: Statement, ConstructableFromDafny<Dafny.ReturnStmt, ReturnStmt> {
  public List<AssignmentRhs> Rhss = new List<AssignmentRhs>();

  private ReturnStmt(Dafny.ReturnStmt retStmtDafny) {
    Rhss.AddRange(retStmtDafny.Rhss.Select(AssignmentRhs.FromDafny));
  }

  public static ReturnStmt FromDafny(Dafny.ReturnStmt dafnyNode) {
    return new ReturnStmt(dafnyNode);
  }
}

public class VarDeclStmt
: Statement, ConstructableFromDafny<Dafny.VarDeclStmt, VarDeclStmt> {
  public List<LocalVariable> Locals = new List<LocalVariable>();
  public ConcreteUpdateStatement? Update;

  private VarDeclStmt(Dafny.VarDeclStmt vdStmt) {
    Locals.AddRange(vdStmt.Locals.Select(LocalVariable.FromDafny));
    Update = vdStmt.Update == null ? null : ConcreteUpdateStatement.FromDafny(vdStmt.Update);
  }

  public static VarDeclStmt FromDafny(Dafny.VarDeclStmt dafnyNode) {
    return new VarDeclStmt(dafnyNode);
  }
}
 
// Method calls
// TODO: CallStmt seems to not be used currently, as method calls tend to be 
// nested in UpdateStmt which we translate by using the pre-resolved Rhss which
// gets the ApplySuffix class. CallStmt are only created after resolution.
public class CallStmt
: Statement, ConstructableFromDafny<Dafny.CallStmt, CallStmt> {
  // TODO: record lhs? (i.e the expressions which are assigned the return values
  // of the method)
  public MemberSelectExpr Callee { get; set; }
  public ArgumentBindings ArgumentBindings { get; set; }

  private CallStmt(Dafny.CallStmt csd) {
    Callee = MemberSelectExpr.FromDafny(csd.MethodSelect);
    ArgumentBindings = ArgumentBindings.FromDafny(csd.Bindings);
  }

  public static CallStmt FromDafny(Dafny.CallStmt dafnyNode) {
    return new CallStmt(dafnyNode);
  }
}
