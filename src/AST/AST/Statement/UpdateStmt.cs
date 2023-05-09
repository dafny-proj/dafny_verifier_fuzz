namespace AST;

public abstract class ConcreteUpdateStatement
: Statement, ConstructableFromDafny<Dafny.ConcreteUpdateStatement, ConcreteUpdateStatement> {
  public static ConcreteUpdateStatement FromDafny(Dafny.ConcreteUpdateStatement cuStmtDafny) {
    return cuStmtDafny switch {
      Dafny.UpdateStmt us => UpdateStmt.FromDafny(us),
      _ => throw new NotImplementedException(),
    };
  }
}

public class UpdateStmt
: ConcreteUpdateStatement, ConstructableFromDafny<Dafny.UpdateStmt, UpdateStmt> {

  public static bool IsAssignStmt(Dafny.UpdateStmt usd) {
    if (usd.ResolvedStatements == null) {
      throw new ArgumentException("Requires update statement to be resolved.");
    }
    return usd.ResolvedStatements.All(s => s is Dafny.AssignStmt);
  }

  public static bool IsCallStmt(Dafny.UpdateStmt usd) {
    if (usd.ResolvedStatements == null) {
      throw new ArgumentException("Requires update statement to be resolved.");
    }
    return usd.ResolvedStatements.Count() == 1
      && usd.ResolvedStatements[0] is Dafny.CallStmt;
  }

  public static UpdateStmt FromDafny(Dafny.UpdateStmt dafnyNode) {
    if (IsAssignStmt(dafnyNode)) {
      var assignments = dafnyNode.ResolvedStatements.Select(s => (s as Dafny.AssignStmt)!);
      return AssignStmt.FromDafny(assignments);
    }
    if (IsCallStmt(dafnyNode)) {
      return CallStmt.FromDafny((dafnyNode.ResolvedStatements[0] as Dafny.CallStmt)!);
    }
    throw new NotSupportedException("Unhandled translation for Dafny.UpdateStmt.");
  }
}

public class AssignStmt
: UpdateStmt, ConstructableFromDafny<IEnumerable<Dafny.AssignStmt>, AssignStmt> {
  public class Assignment : Node {
    public Expression Lhs { get; }
    public AssignmentRhs Rhs { get; }
    public Assignment(Expression lhs, AssignmentRhs rhs) {
      Lhs = lhs;
      Rhs = rhs;
    }

    public override IEnumerable<Node> Children => new Node[] { Lhs, Rhs };
    public override Assignment Clone() {
      return new Assignment(Lhs.Clone(), Rhs.Clone());
    }
  }

  public List<Assignment> Assignments = new();
  public List<Expression> Lhss => Assignments.Select(a => a.Lhs).ToList();
  public List<AssignmentRhs> Rhss => Assignments.Select(a => a.Rhs).ToList();

  public AssignStmt(IEnumerable<Assignment> assignments) {
    Assignments.AddRange(assignments);
  }
  public AssignStmt(Expression lhs, AssignmentRhs rhs) {
    Assignments.Add(new Assignment(lhs, rhs));
  }
  private AssignStmt(IEnumerable<Dafny.AssignStmt> asds) {
    foreach (var asd in asds) {
      Assignments.Add(new Assignment(
        Expression.FromDafny(asd.Lhs), AssignmentRhs.FromDafny(asd.Rhs)));
    }
  }

  public static AssignStmt FromDafny(IEnumerable<Dafny.AssignStmt> dafnyNodes) {
    return new AssignStmt(dafnyNodes);
  }

  public override IEnumerable<Node> Children => Assignments.SelectMany(a => a.Children);
  public override Statement Clone() {
    return new AssignStmt(Assignments.Select(a => a.Clone()));
  }
}

public class CallStmt
: UpdateStmt, ConstructableFromDafny<Dafny.CallStmt, CallStmt> {
  public List<Expression> Lhs = new();
  public MemberSelectExpr Callee { get; set; }
  public ArgumentBindings ArgumentBindings { get; set; }

  public CallStmt(IEnumerable<Expression> lhs, MemberSelectExpr callee, ArgumentBindings args) {
    Lhs.AddRange(lhs);
    Callee = callee;
    ArgumentBindings = args;
  }

  private CallStmt(Dafny.CallStmt csd)
  : this(csd.Lhs.Select(Expression.FromDafny),
    MemberSelectExpr.FromDafny(csd.MethodSelect),
    ArgumentBindings.FromDafny(csd.Bindings)) { }

  public static CallStmt FromDafny(Dafny.CallStmt dafnyNode) {
    return new CallStmt(dafnyNode);
  }

  public override IEnumerable<Node> Children {
    get {
      foreach (var l in Lhs) {
        yield return l;
      }
      yield return Callee;
      yield return ArgumentBindings;
    }
  }
  public override Statement Clone() {
    return new CallStmt(
      Lhs.Select(l => l.Clone()), Callee.Clone(), ArgumentBindings.Clone());
  }
}