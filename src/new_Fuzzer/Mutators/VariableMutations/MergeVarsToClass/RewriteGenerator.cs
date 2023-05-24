namespace Fuzzer_new;

public partial class MergeVarsToClassRewriter {
  private ClassDecl GenClassFromVars(IEnumerable<LocalVar> vars) {
    // Generate class skeleton.
    var cls = ClassDecl.Skeleton(gen.GenClassName());
    // Populate class with a field for each variable.
    List<FieldDecl> fields = new();
    foreach (var v in vars) {
      var ty = v.HasExplicitType() ? v.ExplicitType! : v.Type;
      Contract.Assert(ty is not TypeProxy);
      var fd = new FieldDecl(enclosingDecl: cls, name: v.Name, type: ty);
      varToField.Add(v, fd);
      cls.AddMember(fd);
    }
    // For now, assume no constructors.
    return cls;
  }

  private LocalVar GenClassInstance(ClassDecl cls) {
    var type = new UserDefinedType(cls);
    return new LocalVar(this.gen.GenVarName(), type, type);
  }

  private IdentifierExpr GenClassInstanceIdent() {
    return new IdentifierExpr(clsInstance);
  }

  private MemberSelectExpr GenFieldRefOfVar(LocalVar v) {
    return new MemberSelectExpr(GenClassInstanceIdent(), GetFieldDeclOfVar(v));
  }

}