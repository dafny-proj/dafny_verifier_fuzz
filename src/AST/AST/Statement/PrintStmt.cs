namespace AST;

public class PrintStmt
: Statement, ConstructableFromDafny<Dafny.PrintStmt, PrintStmt> {
  public List<Expression> Args = new();

  public PrintStmt(IEnumerable<Expression> args) {
    Args.AddRange(args);
  }

  private PrintStmt(Dafny.PrintStmt psd)
  : this(psd.Args.Select(Expression.FromDafny)) { }

  public static PrintStmt FromDafny(Dafny.PrintStmt dafnyNode) {
    return new PrintStmt(dafnyNode);
  }

  public override Statement Clone() {
    return new PrintStmt(Args.Select(a => a.Clone()));
  }
}