namespace Fuzzer;

public class VarClass {
  private string ClassName { get; }
  private string InstanceName { get; }
  private ClassDecl ClassDecl { get; }
  private Constructor ConstructorDecl { get; }
  private Dictionary<string, Field> Fields = new();

  public VarClass(List<VarDecl> vars) {
    ClassName = GenClassName();
    InstanceName = GenInstanceName();
    ClassDecl = new ClassDecl(ClassName);
    foreach (var v in vars) {
      var field = new Field(v.Name, v.Type);
      Fields.Add(v.Name, field);
      ClassDecl.AddMember(field);
    }
    ConstructorDecl = new Constructor(ClassDecl);
    ClassDecl.AddMember(ConstructorDecl);
  }

  public bool ContainsVar(string name) {
    return Fields.ContainsKey(name);
  }

  public Type GenClassType() {
    return new UserDefinedType(ClassName);
  }

  public ClassDecl GenClassDecl() {
    return ClassDecl;
  }

  public IdentifierExpr GenInstanceIdent() {
    return new IdentifierExpr(InstanceName, GenClassType());
  }

  public VarDeclStmt GenInstanceDecl() {
    var constructorCallExpr = new MemberSelectExpr(null, ConstructorDecl, GenClassType(), "_ctor", false);
    var constructorCall = new ConstructorCallRhs(
      new CallStmt(new[] { GenInstanceIdent() }, constructorCallExpr, ArgumentBindings.Empty()));
    return new VarDeclStmt(new VarDecl(InstanceName, GenClassType(), constructorCall));
  }

  public MemberSelectExpr GenField(string name) {
    var field = Fields[name];
    return new MemberSelectExpr(
      GenInstanceIdent(), field, field.Type, field.Name, false);
  }

  private string GenClassName() {
    return "C";
  }

  private string GenInstanceName() {
    return "c";
  }
}