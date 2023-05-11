namespace Fuzzer;

public class Scope {
  // The variables declared in this scope.
  // Not inclusive of variables declared in the parent or any child scope.
  public Dictionary<string, VarDecl> Vars = new();
  public Scope Parent { get; }
  public List<Scope> Children = new();
  // The node at which the scope was introduced.
  public Node Node { get; }

  // This constructor should only be used for the top-level program scope
  // with no parent. The parent is then set to itself. 
  private Scope(Program p) {
    Parent = this;
    Node = p;
  }
  public static Scope CreateProgramScope(Program p) {
    return new Scope(p);
  }

  public Scope(Scope parent, Node node) {
    Parent = parent;
    Node = node;
  }

  public Scope AddChild(Node n) {
    var child = new Scope(parent: this, node: n);
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
  public List<Scope> Scopes { get; }
  private Scope CurrentScope;

  public ScopeBuilder(Program p) {
    ProgramScope = Scope.CreateProgramScope(p);
    Scopes = new() { ProgramScope };
    CurrentScope = ProgramScope;
  }

  public void Build() {
    VisitNode(CurrentScope.Node);
  }

  private void PushScope(Node n) {
    CurrentScope = CurrentScope.AddChild(n);
    Scopes.Add(CurrentScope);
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
    PushScope(bs);
    VisitChildren(bs);
    PopScope();
  }

  public void VisitVarDeclStmt(VarDeclStmt vds) {
    vds.Decls.ForEach(vd => CurrentScope.AddVarDecl(vd));
    // So far, no need to dive into individual VarDecls.
  }

}