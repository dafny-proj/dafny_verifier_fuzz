namespace Fuzzer_new;

// Requires variables to have known types (i.e. not a TypeProxy).
// Requires variable types to be auto-initialisable types.
/// <summary>
/// var x, y;
/// Rewrites to
/// ```
/// class C {
///   var x;
///   var y;
/// }
/// ```
/// </summary>
public partial class MergeVarsToClassMutation {
  public List<LocalVar> VarsToMerge;
  public ModuleDecl EnclosingModule;
  public BlockStmt EnclosingScope;
  public IGenerator Generator;

  public MergeVarsToClassMutation(IEnumerable<LocalVar> varsToMerge,
  ModuleDecl enclosingModule, BlockStmt enclosingScope, IGenerator generator) {
    VarsToMerge = varsToMerge.ToList();
    EnclosingModule = enclosingModule;
    EnclosingScope = enclosingScope;
    Generator = generator;
  }

  public void Apply() {
    MergeVarsToClassRewriter.ExecuteMutation(this);
  }
}

public partial class MergeVarsToClassRewriter {
  private Dictionary<LocalVar, FieldDecl> varToField = new();
  private ClassDecl cls;
  private LocalVar clsInstance;
  private ModuleDecl enclosingModule;
  private BlockStmt enclosingScope;
  private IGenerator gen;
  private List<Task> rewriteTasks = new();

  public static void ExecuteMutation(MergeVarsToClassMutation m) {
    new MergeVarsToClassRewriter(m).Rewrite();
  }

  private MergeVarsToClassRewriter(MergeVarsToClassMutation m) {
    this.gen = m.Generator;
    this.enclosingModule = m.EnclosingModule;
    this.enclosingScope = m.EnclosingScope;
    this.cls = GenClassFromVars(m.VarsToMerge);
    this.clsInstance = GenClassInstance(this.cls);
  }

  private void Rewrite() {
    // Insert class declaration in the enclosing module.
    this.enclosingModule.AddDecl(this.cls);
    // Insert class instance declaration in the enclosing scope.
    this.enclosingScope.Prepend(new VarDeclStmt(this.clsInstance));
    // Rewrite all defs and uses to the variable in the enclosing scope.
    VisitNode(this.enclosingScope);
    rewriteTasks.ForEach(t => t.Execute());
  }

  private bool ContainsVar(LocalVar v) => varToField.ContainsKey(v);
  private FieldDecl GetFieldDeclOfVar(LocalVar v) => varToField[v];
}
