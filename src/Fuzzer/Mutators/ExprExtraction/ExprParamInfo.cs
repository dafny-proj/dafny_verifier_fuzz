namespace Fuzzer;

public class ExprParamInfo {
  public List<ExprInfo> Params;
  public List<Expression> Requires;
  public List<Expression> Reads;

  public ExprParamInfo(IEnumerable<ExprInfo>? params_ = null,
  IEnumerable<Expression>? req = null, IEnumerable<Expression>? reads = null) {
    Params = params_ != null ? new(params_) : new();
    Requires = req != null ? new(req) : new();
    Reads = reads != null ? new(reads) : new();
  }

  public void AddRequires(Expression e) => Requires.Add(e);
}

public class ExprParamInfoBuilder {
  public Dictionary<Expression, ExprInfo> ExprInfos;
  public IRandomizer Rand;
  public ExprParamInfoBuilder(Dictionary<Expression, ExprInfo> exprInfos,
  IRandomizer rand) {
    ExprInfos = exprInfos;
    Rand = rand;
  }
  public ExprParamInfo FindParams(Expression e) {
    return VisitExpr(e);
  }

  private ExprInfo GetExprInfo(Expression e) => ExprInfos[e];
  // The entire expression forms a single parameter.
  private ExprParamInfo SelfAsParam(Expression e) {
    return new ExprParamInfo(new[] { GetExprInfo(e) });
  }
  // The expression doesn't need parameters.
  private ExprParamInfo EmptyParam() {
    return new ExprParamInfo();
  }
  private ExprParamInfo MergeParams(IEnumerable<ExprParamInfo> ps) {
    return new ExprParamInfo(
      params_: ps.SelectMany(p => p.Params),
      req: ps.SelectMany(p => p.Requires),
      reads: ps.SelectMany(p => p.Reads));
  }

  private ExprParamInfo VisitExpr(Expression e_) {
    return e_ switch {
      LiteralExpr e => VisitLiteralExpr(e),
      BinaryExpr e => VisitBinaryExpr(e),
      DatatypeUpdateExpr e => VisitDatatypeUpdateExpr(e),
      _ => SelfAsParam(e_),
    };
  }

  private ExprParamInfo VisitLiteralExpr(LiteralExpr e) {
    // A literal can be passed in by parameter or built into the function.
    return Rand.RandBool() ? SelfAsParam(e) : EmptyParam();
  }

  private ExprParamInfo VisitBinaryExpr(BinaryExpr e) {
    if (Rand.RandBool()) {
      return SelfAsParam(e);
    } else {
      // Construct the binary expression from its subexpressions. The parameters 
      // are the combination of parameters required by its subexpressions.
      var params0 = VisitExpr(e.E0);
      var params1 = VisitExpr(e.E1);
      return MergeParams(new[] { params0, params1 });
    }
  }

  private ExprParamInfo VisitDatatypeUpdateExpr(DatatypeUpdateExpr e) {
    if (Rand.RandBool()) {
      return SelfAsParam(e);
    } else {
      // Find the constructors which match the updated fields. The datatype 
      // value updated must match one of the constructors.
      Expression? requires = null;
      var constructors = e.Updates[0].Key.Constructors.AsEnumerable();
      foreach (var u in e.Updates) {
        constructors = constructors.Intersect(u.Key.Constructors);
      }
      foreach (var c in constructors) {
        var constructorCheck
          = NodeFactory.CreateDatatypeConstructorCheck(e.DatatypeValue, c);
        requires = requires == null ? constructorCheck
          : NodeFactory.CreateOrExpr(requires, constructorCheck);
      }
      // Get parameters needed from subexpressions.
      var subexpressions
        = new[] { e.DatatypeValue }.Concat(e.Updates.Select(u => u.Value));
      var subparams = new List<ExprParamInfo>();
      foreach (var se in subexpressions) { subparams.Add(VisitExpr(se)); }
      var params_ = MergeParams(subparams);
      params_.AddRequires(requires!);
      return params_;
    }
  }

}