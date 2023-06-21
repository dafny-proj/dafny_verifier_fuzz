namespace MutantGenerator;

public class ExprExtractionMutation : IMutation {
  public ExprInfo ExprToExtract;
  public ExprExtractionMutation(ExprInfo exprToExtract) {
    ExprToExtract = exprToExtract;
  }
}

public class ExprExtractionMutator : IBasicMutator {
  public IRandomizer Rand;
  public IGenerator Gen;

  public ExprExtractionMutator(IRandomizer rand, IGenerator gen) {
    Rand = rand;
    Gen = gen;
  }

  public bool TryMutateProgram(Program p) {
    var allExprsInfo = ExprInfoBuilder.FindExprInfo(p);
    var selectedExpr = SelectExprToExtract(allExprsInfo);
    if (selectedExpr == null) { return false; }
    var selectedExprInfo = allExprsInfo[selectedExpr];
    var mutation = new ExprExtractionMutation(exprToExtract: selectedExprInfo);
    ApplyMutation(mutation);
    return true;
  }

  private Expression? SelectExprToExtract(Dictionary<Expression, ExprInfo> es) {
    var candidates = new List<Expression>();
    foreach (var e in es.Keys) {
      if (e is (WildcardExpr or StaticReceiverExpr)) { continue; }
      if (e.Type is BasicType) { candidates.Add(e); }
      // // Skip expressions whose types are not known.
      // if (e.Type is TypeProxy) { continue; }
      // candidates.Add(e);
    }
    // Choose a random valid expression to extract.
    return Rand.RandElement<Expression>(candidates);
  }

  // TODO: Make a parent class covering all declarations with members.
  // For now, just get the default class of the expression's enclosing module.
  private ClassDecl SelectFunctionInjectionPoint(ExprInfo e) {
    return e.EnclosingModule.GetOrCreateDefaultClass();
  }

  public void ApplyMutation(ExprExtractionMutation m) {
    var exprInfo = m.ExprToExtract;
    var exprToExtract = exprInfo.E;
    // Scan expression to select parameters and build function body and spec.
    var functionBuilder = new FunctionBuilder(Rand, Gen);
    var functionData = functionBuilder.BuildFromExpression(exprToExtract);
    // Get function injection point.
    var cls = functionBuilder.ThisClass
      ?? SelectFunctionInjectionPoint(exprInfo);
    // Get function parameters.
    var exprsToParams = functionData.Params;
    var params_ = exprsToParams.Select(ep => ep.Value);
    // Make function signature.
    var function = new FunctionDecl(
      enclosingDecl: cls,
      name: Gen.GenFunctionName(),
      ins: params_,
      resultType: exprToExtract.Type);
    // Attach function body and specifications.
    function.Body = functionData.E;
    function.Precondition = new Specification(
      Specification.Type.Precondition, functionData.Requires);
    function.Reads = new Specification(
      Specification.Type.ReadFrame, functionData.Reads);
    // Inject function declaration.
    cls.AddMember(function);
    // Replace expression at extraction site with a function call.
    var receiver = functionBuilder.ThisObject
      ?? new ImplicitStaticReceiverExpr(cls);
    var call = new FunctionCallExpr(
      callee: new MemberSelectExpr(receiver, function),
      arguments: exprsToParams.Select(ep => ep.Key));
    exprInfo.Parent.ReplaceChild(exprToExtract, call);
  }
}
