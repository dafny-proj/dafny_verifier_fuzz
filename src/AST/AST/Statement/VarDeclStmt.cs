using System.Diagnostics.Contracts;

namespace AST;

public abstract class VarDeclInitialiser : Node { }

public class AssignmentInitialiser : VarDeclInitialiser {
  public AssignmentRhs Value { get; private set; }

  public AssignmentInitialiser(AssignmentRhs value) {
    Value = value;
  }

  public override IEnumerable<Node> Children => new[] { Value };
}

public class OtherInitialiser : VarDeclInitialiser {
  public OtherInitialiser() {
    throw new NotImplementedException();
  }
}

public class VarDecl : Node {
  public string Name { get; private set; }
  public Type? ExplicitType { get; private set; }
  public Type? InferredType { get; private set; }
  public VarDeclInitialiser? Initialiser { get; private set; }

  public VarDecl(string name,
    Type? explicitType = null,
    Type? inferredType = null,
    VarDeclInitialiser? initialiser = null) {
    Name = name;
    ExplicitType = explicitType;
    InferredType = inferredType;
    Initialiser = initialiser;
  }

  public void SetInferredType(Type t) => InferredType = t;
  public void SetInitialiser(VarDeclInitialiser vdi)
    => Initialiser = vdi;

  public override IEnumerable<Node> Children {
    get {
      if (Initialiser != null) {
        yield return Initialiser;
      }
    }
  }
}

public class VarDecls
: Statement, ConstructableFromDafny<Dafny.VarDeclStmt, VarDecls> {
  public List<VarDecl> Decls = new();

  private VarDecls(Dafny.VarDeclStmt vds) {
    vds.Locals.ForEach(l => {
      var et = l.IsTypeExplicit ? Type.FromDafny(l.OptionalType) : null;
      Decls.Add(new VarDecl(l.Name, explicitType: et));
    });
    var initStmt = vds.Update == null ? null : ConcreteUpdateStatement.FromDafny(vds.Update);
    if (initStmt != null) {
      if (initStmt is UpdateStmt us) {
        // Lhss and Rhss should match up exactly in a declaration statement.
        Contract.Assert(Decls.Count == us.Lhss.Count);
        Contract.Assert(Decls.Count == us.Rhss.Count);
        for (var i = 0; i < Decls.Count; i++) {
          // TODO: Is this the correct inferred type? Or should we try and get type from Rhs?
          Decls[i].SetInferredType(us.Lhss[i].Type);
          Decls[i].SetInitialiser(new AssignmentInitialiser(us.Rhss[i]));
        }
      } else {
        throw new NotImplementedException($"VarDecls: Unhandled initialiser from {initStmt.GetType()}.");
      }
    }
  }

  public static VarDecls FromDafny(Dafny.VarDeclStmt dafnyNode) {
    return new VarDecls(dafnyNode);
  }

  public bool HasInitialiser() {
    return Decls[0].Initialiser != null;
  }

  public List<VarDeclInitialiser> GetDeclInitialisers() {
    List<VarDeclInitialiser> inits = new();
    if (HasInitialiser()) {
      Contract.Assert(Decls.TrueForAll(d => d.Initialiser != null));
      inits.AddRange(Decls.Select(d => d.Initialiser!));
    }
    return inits;
  }

  public override IEnumerable<Node> Children => Decls;
}

public class VarDeclStmt
: Statement, ConstructableFromDafny<Dafny.VarDeclStmt, VarDeclStmt> {
  public override IEnumerable<Node> Children {
    get {
      var children = Locals;
      if (Update != null) {
        children.Append<Node>(Update);
      }
      return children;
    }
  }
  public List<LocalVariable> Locals = new List<LocalVariable>();
  public ConcreteUpdateStatement? Update;

  public VarDeclStmt(LocalVariable lv, ConcreteUpdateStatement us) {
    Locals.Add(lv);
    Update = us;
  }

  private VarDeclStmt(Dafny.VarDeclStmt vdStmt) {
    Locals.AddRange(vdStmt.Locals.Select(LocalVariable.FromDafny));
    Update = vdStmt.Update == null ? null : ConcreteUpdateStatement.FromDafny(vdStmt.Update);
  }

  public static VarDeclStmt FromDafny(Dafny.VarDeclStmt dafnyNode) {
    return new VarDeclStmt(dafnyNode);
  }
}