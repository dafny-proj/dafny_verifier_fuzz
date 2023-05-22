using System.Diagnostics.Contracts;

namespace AST_new.Cloner;

public partial class ASTCloner {
  private Dictionary<Declaration, Declaration?> DeclarationsToClone = new();
  private Dictionary<Declaration, Declaration> ClonedDeclarations = new();
  private void MarkDeclCloneSkeleton(Declaration d, Declaration c) {
    Contract.Assert(DeclarationsToClone.ContainsKey(d));
    DeclarationsToClone[d] = c;
  }
  private void MarkDeclCloned(Declaration d, Declaration c) {
    Contract.Assert(DeclarationsToClone.ContainsKey(d));
    ClonedDeclarations[d] = c;
  }
  private bool HasClonedDecl(Declaration d) {
    return ClonedDeclarations.ContainsKey(d);
  }
  private Declaration GetClonedDecl(Declaration d) {
    return ClonedDeclarations[d];
  }
  private bool HasSkeletonDeclClone(Declaration d) {
    return DeclarationsToClone.ContainsKey(d) && DeclarationsToClone[d] != null;
  }
  private Declaration GetSkeletonDeclClone(Declaration d) {
    return DeclarationsToClone[d]!;
  }

  private Dictionary<Variable, Variable> ClonedVariables = new();
  private Variable GetOrCreateVariableClone(Variable v) {
    if (!ClonedVariables.ContainsKey(v)) {
      ClonedVariables.Add(v, CreateVariableClone(v));
    }
    return ClonedVariables[v];
  }

  public static T Clone<T>(Node n) where T : Node {
    var cloner = new ASTCloner();
    cloner.IdentifyDeclsToClone(n);
    return (T)cloner.CloneNode(n);
  }

  private void IdentifyDeclsToClone(Node n) {
    if (n is Declaration d) {
      DeclarationsToClone.Add(d, null);
    }
    foreach (var c in n.Children) {
      IdentifyDeclsToClone(c);
    }
  }

  private Node CloneNode(Node n) {
    return n switch {
      Program p => CloneProgram(p),
      Declaration d => CloneDeclaration(d),
      Statement s => CloneStatement(s),
      Expression e => CloneExpression(e),
      AssignmentRhs a => CloneAssignmentRhs(a),
      Type t => CloneType(t),
      Variable v => CloneVariable(v),
      Specification s => CloneSpecification(s)!,
      ExpressionPair p => CloneExpressionPair(p),
      AssignmentPair p => CloneAssignmentPair(p),
      _ => throw new UnsupportedNodeCloningException(n),
    };
  }

  private Program CloneProgram(Program p) {
    return new Program(CloneModuleDecl(p.ProgramModule));
  }
}
