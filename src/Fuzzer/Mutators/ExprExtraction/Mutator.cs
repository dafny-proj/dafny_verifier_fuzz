namespace Fuzzer;

public class ExprExtractionMutation : IMutation {
  public ExprInfo ExprToExtract;
  public ClassDecl FunctionInjectionPoint;
  public ExprExtractionMutation(ExprInfo exprToExtract,
  ClassDecl functionInjectionPoint) {
    ExprToExtract = exprToExtract;
    FunctionInjectionPoint = functionInjectionPoint;
  }
}

public class ExprExtractionMutator : IBasicMutator {
  public IRandomizer Rand;
  public IGenerator Gen;

  public ExprExtractionMutator(IRandomizer rand, IGenerator gen) {
    Rand = rand;
    Gen = gen;
  }

  // Stage 1: Extract a expression into a static function of the default class.
  public bool TryMutateProgram(Program p) {
    var allExprsInfo = ExprInfoBuilder.FindExprInfo(p);
    var selectedExpr = SelectExprToExtract(allExprsInfo);
    if (selectedExpr == null) { return false; }
    var selectedExprInfo = allExprsInfo[selectedExpr];
    var mutation = new ExprExtractionMutation(
      exprToExtract: selectedExprInfo,
      functionInjectionPoint: SelectFunctionInjectionPoint(selectedExprInfo));
    ApplyMutation(mutation);
    return true;
  }

  private Expression? SelectExprToExtract(Dictionary<Expression, ExprInfo> es) {
    var candidates = new List<Expression>();
    foreach (var e in es.Keys) {
      if (e is (WildcardExpr or StaticReceiverExpr)) { continue; }
      // Skip expressions whose types are not known.
      if (e.Type is TypeProxy) { continue; }
      candidates.Add(e);
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
    var cls = m.FunctionInjectionPoint;
    var exprInfo = m.ExprToExtract;
    var exprToExtract = exprInfo.E;
    var functionData = new FunctionBuilder(Rand, Gen).BuildFromExpression(exprToExtract);
    var exprsToParams = functionData.Params;
    var params_ = exprsToParams.Select(ep => ep.Value);
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
    var call = new FunctionCallExpr(
      callee: new MemberSelectExpr(new ImplicitStaticReceiverExpr(cls), function),
      arguments: exprsToParams.Select(ep => ep.Key));
    exprInfo.Parent.ReplaceChild(exprToExtract, call);
  }

  private void
  RewriteExprConvertedParams(Dictionary<ExprInfo, Formal> exprsToParams) {
    // By construction, the expressions chosen as params are in separate lines 
    // of hierarchy, i.e. each expression is neither a transitive child/parent
    // of each other.
    foreach (var ep in exprsToParams) {
      var e = ep.Key;
      var p = ep.Value;
      e.Parent.ReplaceChild(e.E, new IdentifierExpr(p));
    }
  }
}
