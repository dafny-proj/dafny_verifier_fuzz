namespace Fuzzer;

public partial class MergeVarsToClassMutationRewriter {
  private Dictionary<LocalVar, FieldDecl> varToField = new();
  private ClassDecl cls;
  private Type clsType;
  private LocalVar clsInstance;
  private ModuleDecl enclosingModule;
  private BlockStmt enclosingScope;
  private IGenerator gen;
  private List<Task> rewriteTasks = new();

  public MergeVarsToClassMutationRewriter(
  MergeVarsToClassMutation m, IGenerator g) {
    this.gen = g;
    this.enclosingModule = m.EnclosingModule;
    this.enclosingScope = m.EnclosingScope;
    this.cls = GenClassFromVars(m.Vars);
    this.clsType = new UserDefinedType(this.cls);
    this.clsInstance = GenClassInstance(this.clsType);
  }

  public void Rewrite() {
    // Insert class declaration in the enclosing module.
    this.enclosingModule.PrependDecl(this.cls);
    // Insert class instance declaration in the enclosing scope.
    this.enclosingScope.Prepend(GenClassInstanceDecl());
    // Rewrite all defs and uses to the variable in the enclosing scope.
    VisitNode(this.enclosingScope);
    rewriteTasks.ForEach(t => t.Execute());
  }

  private bool ContainsVar(LocalVar v) => varToField.ContainsKey(v);
  private FieldDecl GetFieldDeclOfVar(LocalVar v) => varToField[v];
  private LocalVar? TryGetAffectedVar(Expression e) {
    if (e is IdentifierExpr i && i.Var is LocalVar lv && ContainsVar(lv)) {
      return lv;
    }
    return null;
  }

  private bool NotAutoInitialisable(Type t) {
    if (t is BasicType or BuiltInType or NullableType or CollectionType) {
      return false;
    }
    // TODO: Track auto-init info in types.
    // This is not exactly accurate, but until there is better tracking of which 
    // types are auto initialisable, use a blanket case.
    return true;
  }

  private Type? TryConvertToAutoInitialisable(Type t) {
    if (t is UserDefinedType ut) {
      if (ut.TypeDecl is ClassDecl cd) {
        return new NullableType(cd, ut.GetTypeArgs());
      } else if (ut.TypeDecl is DatatypeDecl dd) {
        // Create a None constructor for the datatype.
        var none = new DatatypeConstructorDecl(dd, "DF_None");
        dd.PrependConstructor(none);
        return t;
      }
    }
    // TODO: Handle other cases. An idea is to wrap the type in a class
    // to enable formation of nullable types, or in a datatype with a None case.
    return null;
  }

  private ClassDecl GenClassFromVars(IEnumerable<LocalVar> vars) {
    // Generate class skeleton.
    var cls = ClassDecl.Skeleton(gen.GenClassName());
    // Populate class with a field for each variable.
    foreach (var v in vars) {
      var ty = v.HasExplicitType() ? v.ExplicitType! : v.Type;
      Contract.Assert(ty is not TypeProxy);
      if (NotAutoInitialisable(ty)) {
        ty = TryConvertToAutoInitialisable(ty);
        if (ty == null) {
          // Failed to convert to auto-initialisable type. Skip this var.
          continue;
        }
      }
      var fd = new FieldDecl(enclosingDecl: cls, name: v.Name, type: ty);
      varToField.Add(v, fd);
      cls.AddMember(fd);
    }
    // For now, assume no constructors.
    return cls;
  }

  private LocalVar GenClassInstance(Type clsType) {
    return new LocalVar(this.gen.GenVarName(), clsType, clsType);
  }

  private VarDeclStmt GenClassInstanceDecl() {
    var initialiser = new AssignStmt(new AssignmentPair(
      GenClassInstanceIdent(), new NewObjectRhs(clsType)));
    return new VarDeclStmt(this.clsInstance, initialiser);
  }

  private IdentifierExpr GenClassInstanceIdent() {
    return new IdentifierExpr(clsInstance);
  }

  private MemberSelectExpr GenFieldRefOfVar(LocalVar v) {
    return new MemberSelectExpr(GenClassInstanceIdent(), GetFieldDeclOfVar(v));
  }
}
