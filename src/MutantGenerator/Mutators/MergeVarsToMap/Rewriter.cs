namespace MutantGenerator;

public partial class MergeVarsToMapMutationRewriter {
  private List<LocalVar> vars;
  private LocalVar map;
  private MapType mapType;
  private BlockStmt enclosingScope;
  private IGenerator gen;
  private List<Task> rewriteTasks = new();

  public MergeVarsToMapMutationRewriter(
  MergeVarsToMapMutation m, IGenerator g) {
    this.gen = g;
    this.enclosingScope = m.EnclosingScope;
    this.vars = m.Vars;
    var valueType = GetCommonType(this.vars);
    if (valueType == null) {
      throw new ASTException(
        $"Only variables of the same type can be merged into a map.");
    }
    this.mapType = new MapType(Type.String, valueType);
    this.map = GenMapVariable(mapType);
  }

  public void Rewrite() {
    // Insert map declaration in the enclosing scope.
    this.enclosingScope.Prepend(GenMapVariableDecl());
    // Rewrite all defs and uses to the variable in the enclosing scope.
    VisitNode(this.enclosingScope);
    rewriteTasks.ForEach(t => t.Execute());
  }

  private Type? GetCommonType(List<LocalVar> vars) {
    if (vars.Count() > 0) {
      var type = vars[0].Type;
      var common = vars.All(v => type.Equals(v.Type));
      return common ? type : null;
    }
    return null;
  }

  private bool ContainsVar(LocalVar v) => vars.Contains(v);
  private LocalVar? TryGetAffectedVar(Expression e) {
    if (e is IdentifierExpr i && i.Var is LocalVar lv && ContainsVar(lv)) {
      return lv;
    }
    return null;
  }

  private LocalVar GenMapVariable(MapType mt) {
    return new LocalVar(this.gen.GenVarName(), mt, mt);
  }

  private VarDeclStmt GenMapVariableDecl() {
    return new VarDeclStmt(map, new AssignStmt(
      new AssignmentPair(GenMapIdent(), new ExprRhs(GenEmptyMap()))));
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
    return new CollectionElementExpr(collection: GenMapIdent(), 
      index: GenMapIndexForVar(v), type: v.Type);
  }

  // map[]
  public MapDisplayExpr GenEmptyMap() {
    return new MapDisplayExpr(type: mapType);
  }

  // map[v0 := ..., v1 := ..., ...]
  public MapDisplayExpr
  GenMapDisplayForVars(IEnumerable<ExpressionPair> assignments) {
    return new MapDisplayExpr(assignments, type: mapType);
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
