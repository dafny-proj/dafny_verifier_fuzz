namespace AST_new;

public partial class DatatypeValue : Expression {
  public DatatypeConstructorDecl Constructor { get; }
  public List<Expression> ConstructorArguments = new();

  public DatatypeValue(DatatypeConstructorDecl constructor,
  IEnumerable<Expression>? constructorArguments = null) {
    Constructor = constructor;
    if (constructorArguments != null) {
      ConstructorArguments.AddRange(constructorArguments);
    }
  }
}
