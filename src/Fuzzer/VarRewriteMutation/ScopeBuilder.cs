namespace Fuzzer;

public class Scope {
  // The variables declared in this scope.
  // Not inclusive of variables declared in the parent or any child scope.
  public Dictionary<string, VarDecl> Vars = new();
  public Scope Parent;
  public List<Scope> Children = new();

  // This constructor should only be used for the top-level program scope
  // with no parent. The parent is then set to itself. 
  private Scope() {
    Parent = this;
  }
  public static Scope CreateProgramScope() {
    return new Scope();
  }

  public Scope(Scope parent) {
    Parent = parent;
  }

  public Scope AddChild() {
    var child = new Scope(parent: this);
    Children.Add(child);
    return child;
  }

  public Scope GetParent() {
    return Parent;
  }

  public void AddVarDecl(VarDecl vdi) {
    Vars.Add(vdi.Name, vdi);
  }
}

// Currently collects declaration of local variables within functions/methods.
// Other declarations, e.g. parameters, fields, are currently ignored.
public class ScopeBuilder : ASTVisitor {
  public Scope ProgramScope { get; }
  private Scope CurrentScope;

  public ScopeBuilder() {
    ProgramScope = Scope.CreateProgramScope();
    CurrentScope = ProgramScope;
  }

  private void PushScope() {
    CurrentScope = CurrentScope.AddChild();
  }

  private void PopScope() {
    CurrentScope = CurrentScope.GetParent();
  }

  public override void VisitStmt(Statement s) {
    switch (s) {
      case BlockStmt bs:
        VisitBlockStmt(bs);
        break;
      case VarDeclStmt vds:
        VisitVarDeclStmt(vds);
        break;
      default:
        base.VisitStmt(s);
        break;
    }
  }

  public void VisitBlockStmt(BlockStmt bs) {
    PushScope();
    VisitChildren(bs);
    PopScope();
  }

  public void VisitVarDeclStmt(VarDeclStmt vds) {
    vds.Decls.ForEach(vd => CurrentScope.AddVarDecl(vd));
    // So far, no need to dive into individual VarDecls.
  }

}