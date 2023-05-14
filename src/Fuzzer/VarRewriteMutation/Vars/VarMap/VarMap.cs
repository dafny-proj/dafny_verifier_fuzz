using System.Diagnostics.Contracts;

namespace Fuzzer;

public class VarMap {
  string MapName { get; set; }
  MapType MapType { get; set; }
  List<string> Vars = new();

  public VarMap(List<VarDecl> vars) {
    var valueT = GetCommonType(vars);
    if (valueT == null) {
      throw new ArgumentException(
        $"Only variables of the same type can be merged into a map.");
    }
    MapName = GenMapName();
    // The keys are the namestrings of the variables.
    // The common type of the variables forms the value type.
    MapType = new MapType(kt: Type.String, vt: valueT);
    Vars.AddRange(vars.Select(v => v.Name));
  }

  public Type? GetCommonType(List<VarDecl> vars) {
    if (vars.Count() > 0) {
      var type = vars[0].Type;
      var common = vars.All(v => type == v.Type);
      return common ? type : null;
    }
    return null;
  }

  public bool ContainsVar(string name) {
    return Vars.Contains(name);
  }

  // `var m: map<string, valueT> := map[]`
  public VarDeclStmt GenMapVarDecl() {
    MapDisplayExpr emptyMap = new MapDisplayExpr(MapType);
    VarDecl mapDecl = new VarDecl(MapName, MapType, init: emptyMap);
    return new VarDeclStmt(mapDecl);
  }

  // TODO: Generate multiple identifiers or have a singleton identifier?
  public IdentifierExpr GenMapVarIdent() {
    return new IdentifierExpr(MapName, MapType);
  }

  // `a` -> `"a"`
  public StringLiteralExpr GenMapVarIndex(string var) {
    Contract.Requires(this.ContainsVar(var));
    return new StringLiteralExpr(var);
  }

  // `a` -> `m["a"]`
  public CollectionSelectExpr GenMapVarElement(string var) {
    Contract.Requires(this.ContainsVar(var));
    return CollectionSelectExpr.Element(
      GenMapVarIdent(), GenMapVarIndex(var), MapType.ValueType);
  }

  // `a := 1` -> `m := m["a" := 1]`
  public AssignStmt.Assignment GenMapVarAssignment(string var, Expression value) {
    return new AssignStmt.Assignment(GenMapVarIdent(), new ExprRhs(
      new CollectionUpdateExpr(GenMapVarIdent(), GenMapVarIndex(var), value)));
  }

  // TODO: Handle random, non-colliding name generation.
  private string GenMapName() {
    return "m";
  }
}