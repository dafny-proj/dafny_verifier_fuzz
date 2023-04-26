using System.Diagnostics.Contracts;

namespace AST;

public class ArgumentBindings
: Node, ConstructableFromDafny<Dafny.ActualBindings, ArgumentBindings> {
  // Arguments provided in the program text and the parameters they correspond to.
  public List<ArgumentBinding> providedArguments = new List<ArgumentBinding>();
  // Arguments provided in the program text + default arguments, arguments are 
  // given in positional order of parameters hence binding is not required here.
  public List<Expression>? allArguments {get; set;}

  private ArgumentBindings(Dafny.ActualBindings abd) {
    Contract.Requires(abd.WasResolved); // For abd.Arguments to be available
    providedArguments.AddRange(abd.ArgumentBindings.Select(ArgumentBinding.FromDafny));
    if (abd.Arguments != null) {
      allArguments = abd.Arguments.Select(Expression.FromDafny).ToList();
    }
  }

  public static ArgumentBindings FromDafny(Dafny.ActualBindings dafnyNode) {
    return new ArgumentBindings(dafnyNode);
  }
}

public class ArgumentBinding
: Node, ConstructableFromDafny<Dafny.ActualBinding, ArgumentBinding> {
  public string? FormalParameterName { get; }
  public Expression Argument { get; set; }

  public bool IsPositional => FormalParameterName == null;

  private ArgumentBinding(Dafny.ActualBinding abd) {
    FormalParameterName = abd.FormalParameterName == null ? null
      : abd.FormalParameterName.val;
    Argument = Expression.FromDafny(abd.Actual);
  }

  public static ArgumentBinding FromDafny(Dafny.ActualBinding dafnyNode) {
    return new ArgumentBinding(dafnyNode);
  }
}