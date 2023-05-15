using System.Diagnostics.Contracts;

namespace AST;

public class ArgumentBindings
: Node, ConstructableFromDafny<Dafny.ActualBindings, ArgumentBindings> {
  // TODO: refactor providedArguments and allArguments into a single structure
  public override IEnumerable<Node> Children => ProvidedArgs;

  // Arguments provided in the program text and the parameters they correspond to.
  public List<ArgumentBinding> ProvidedArgs = new List<ArgumentBinding>();
  // Arguments provided in the program text + default arguments, arguments are 
  // given in positional order of parameters hence binding is not required here.
  public List<Expression>? AllArgs { get; set; }

  public ArgumentBindings() { }

  public ArgumentBindings(IEnumerable<ArgumentBinding> args) {
    ProvidedArgs.AddRange(args);
  }

  private ArgumentBindings(Dafny.ActualBindings abd) {
    Contract.Requires(abd.WasResolved); // For abd.Arguments to be available
    ProvidedArgs.AddRange(abd.ArgumentBindings.Select(ArgumentBinding.FromDafny));
    if (abd.Arguments != null) {
      AllArgs = abd.Arguments.Select(Expression.FromDafny).ToList();
    }
  }

  public static ArgumentBindings FromDafny(Dafny.ActualBindings dafnyNode) {
    return new ArgumentBindings(dafnyNode);
  }

  public static ArgumentBindings Empty() {
    return new ArgumentBindings();
  }

  public override ArgumentBindings Clone() {
    var clone = new ArgumentBindings(ProvidedArgs.Select(a => a.Clone()));
    clone.AllArgs = AllArgs?.Select(a => a.Clone()).ToList();
    return clone;
  }
}

public class ArgumentBinding
: Node, ConstructableFromDafny<Dafny.ActualBinding, ArgumentBinding> {
  public override IEnumerable<Node> Children => new[] { Argument };

  public string? FormalParameterName { get; }
  public Expression Argument { get; set; }

  public bool IsPositional => FormalParameterName == null;

  public ArgumentBinding(Expression arg, string? paramName = null) {
    Argument = arg;
    FormalParameterName = paramName;
  }

  private ArgumentBinding(Dafny.ActualBinding abd)
  : this(Expression.FromDafny(abd.Actual), abd.FormalParameterName?.val) { }

  public static ArgumentBinding FromDafny(Dafny.ActualBinding dafnyNode) {
    return new ArgumentBinding(dafnyNode);
  }

  public override ArgumentBinding Clone() {
    return new ArgumentBinding(Argument.Clone(), FormalParameterName);
  }
}