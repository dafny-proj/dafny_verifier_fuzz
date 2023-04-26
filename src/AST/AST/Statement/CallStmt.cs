namespace AST;

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