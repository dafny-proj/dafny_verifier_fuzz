namespace AST_new;

public partial class DatatypeValueExpr : Expression {
  public DatatypeConstructorDecl Constructor { get; }
  public List<Expression> ConstructorArguments = new();

  public string ConstructorName => Constructor.Name;
  public string DatatypeName => Constructor.DatatypeName;
  public bool HasArguments() => ConstructorArguments.Count > 0;

  public DatatypeValueExpr(DatatypeConstructorDecl constructor,
  IEnumerable<Expression>? constructorArguments = null) {
    Constructor = constructor;
    if (constructorArguments != null) {
      ConstructorArguments.AddRange(constructorArguments);
    }
  }
}
