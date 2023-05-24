namespace Fuzzer_new;

public partial class MergeVarsToMapRewriter {
  private LocalVar GenMapVariable(MapType mt) {
    return new LocalVar(this.gen.GenVarName(), mt, mt);
  }

  // m
  private IdentifierExpr GenMapIdent() {
    return new IdentifierExpr(map);
  }

  // "v"
  private StringLiteralExpr GenMapIndexForVar(LocalVar v) {
    Contract.Assert(ContainsVar(v));
    return new StringLiteralExpr(v.Name);
  }

  // m[v]
  private CollectionSelectExpr GenMapElementForVar(LocalVar v) {
    Contract.Assert(ContainsVar(v));
    return new CollectionElementExpr(
      collection: GenMapIdent(), index: GenMapIndexForVar(v));
  }

  // map[]
  public MapDisplayExpr GenEmptyMap() {
    return new MapDisplayExpr();
  }

  // map[v0 := ..., v1 := ..., ...]
  public MapDisplayExpr
  GenMapDisplayForVars(IEnumerable<ExpressionPair> assignments) {
    return new MapDisplayExpr(assignments);
  }

  // m[v := ...]
  public CollectionUpdateExpr GenMapUpdateValue(LocalVar v, Expression value) {
    return new CollectionUpdateExpr(
      collection: GenMapIdent(), index: GenMapIndexForVar(v), value: value);
  }
  private CollectionUpdateExpr GenMapUpdateValue(ExpressionPair ep) {
    return new CollectionUpdateExpr(GenMapIdent(), ep.Key, ep.Value);
  }

  // m + map[v0 := ..., v1 := ..., ...]
  private Expression GenMapUpdateValues(List<ExpressionPair> eps) {
    return new BinaryExpr(BinaryExpr.Opcode.Add,
      GenMapIdent(), GenMapDisplayForVars(eps));
  }

  // m := e
  private AssignmentPair GenMapAssignment(Expression e) {
    return new AssignmentPair(GenMapIdent(), new ExprRhs(e));
  }

}