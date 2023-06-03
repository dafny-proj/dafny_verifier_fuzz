namespace Fuzzer;

public class ExprExtractionMutation : IMutation {
  public ExprInfo ExprToExtract;
  public List<ExprInfo> ExprsForParams;
  public ClassDecl FunctionInjectionPoint;
  public ExprExtractionMutation(ExprInfo exprToExtract,
  List<ExprInfo> exprForParams, ClassDecl functionInjectionPoint) {
    ExprToExtract = exprToExtract;
    ExprsForParams = exprForParams;
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
    var allExprs = ExprInfoBuilder.FindExprInfo(p);
    var selectedExprInfo = SelectExprToExtract(allExprs);
    if (selectedExprInfo == null) { return false; }
    var mutation = new ExprExtractionMutation(
      exprToExtract: selectedExprInfo,
      exprForParams: SelectFunctionParams(selectedExprInfo),
      functionInjectionPoint: SelectFunctionInjectionPoint(selectedExprInfo));
    ApplyMutation(mutation);
    return true;
  }

  private ExprInfo? SelectExprToExtract(List<ExprInfo> es) {
    var candidates = new List<ExprInfo>();
    foreach (var e in es) {
      // Skip expressions whose types are not known.
      if (e.E.Type is TypeProxy) { continue; }
      candidates.Add(e);
    }
    // Choose a random valid expression to extract.
    return Rand.RandElement<ExprInfo>(candidates);
  }

  // TODO: Go through the subexpressions as possible parameters.
  // For now, select the entire function body as the parameter, i.e. producing a
  // identity function.
  private List<ExprInfo> SelectFunctionParams(ExprInfo e) {
    var params_ = new List<ExprInfo>();
    params_.Add(e);
    return params_;
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
    var exprsToParams = new Dictionary<ExprInfo, Formal>();
    foreach (var e in m.ExprsForParams) {
      exprsToParams.Add(e, new Formal(Gen.GenFormalName(), e.E.Type));
    }
    var params_ = exprsToParams.Select(ep => ep.Value);
    var function = new FunctionDecl(
      enclosingDecl: cls,
      name: Gen.GenFunctionName(),
      ins: params_,
      resultType: exprToExtract.Type);
    // Attach and rewrite function body.
    var oldParent = exprInfo.Parent;
    exprInfo.Parent = function;
    function.Body = exprToExtract;
    RewriteExprConvertedParams(exprsToParams);
    // Inject function declaration.
    cls.AddMember(function);
    // Replace expression at extraction site with a function call.
    var call = new FunctionCallExpr(
      callee: new MemberSelectExpr(new ImplicitStaticReceiverExpr(cls), function),
      arguments: exprsToParams.Select(ep => ep.Key.E));
    oldParent.ReplaceChild(exprToExtract, call);
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
