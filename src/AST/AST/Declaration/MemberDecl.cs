namespace AST;

public class MemberDecl
: Declaration, ConstructableFromDafny<Dafny.MemberDecl, MemberDecl> {
  public static MemberDecl FromDafny(Dafny.MemberDecl dafnyNode) {
    return dafnyNode switch {
      Dafny.Method method => Method.FromDafny(method),
      Dafny.Function func => Function.FromDafny(func),
      _ => throw new NotImplementedException(),
    };

  }
}

public class Method
: MemberDecl, ConstructableFromDafny<Dafny.Method, Method> {
  // TODO: TypeArgs
  public string Name { get; set; }
  public BlockStmt Body { get; set; }
  private Specification<Dafny.Expression, Expression> _Decreases { get; set; }
  public Specification<Dafny.Expression, Expression> AllDecreases {
    get => _Decreases;
  }
  public Specification<Dafny.Expression, Expression> ProvidedDecreases {
    get => _Decreases.GetProvided();
  }
  public List<Formal> Ins = new List<Formal>();
  public List<Formal> Outs = new List<Formal>();
  public List<AttributedExpression> Req = new List<AttributedExpression>();
  public List<AttributedExpression> Ens = new List<AttributedExpression>();
  public Specification<Dafny.FrameExpression, FrameExpression> Mod { get; set; }


  private Method(Dafny.Method methodDafny) {
    Name = methodDafny.Name;
    Body = BlockStmt.FromDafny(methodDafny.Body);
    _Decreases = Specification<Dafny.Expression, Expression>.FromDafny(methodDafny.Decreases);
    Ins.AddRange(methodDafny.Ins.Select(Formal.FromDafny));
    Outs.AddRange(methodDafny.Outs.Select(Formal.FromDafny));
    Req.AddRange(methodDafny.Req.Select(AttributedExpression.FromDafny));
    Ens.AddRange(methodDafny.Ens.Select(AttributedExpression.FromDafny));
    Mod = Specification<Dafny.FrameExpression, FrameExpression>.FromDafny(methodDafny.Mod);
  }
  public static Method FromDafny(Dafny.Method dafnyNode) {
    return new Method(dafnyNode);
  }
}

public class Function
: MemberDecl, ConstructableFromDafny<Dafny.Function, Function> {
  // TODO: TypeArgs, byMethodBody
  public string Name { get; set; }
  public Expression Body { get; set; }
  private Specification<Dafny.Expression, Expression> _Decreases { get; set; }
  public Specification<Dafny.Expression, Expression> AllDecreases {
    get => _Decreases;
  }
  public Specification<Dafny.Expression, Expression> ProvidedDecreases {
    get => _Decreases.GetProvided();
  }
  public List<Formal> Ins = new List<Formal>();
  public Formal? Out { get; set; }
  public Type OutType { get; set; }
  public List<AttributedExpression> Req = new List<AttributedExpression>();
  public List<AttributedExpression> Ens = new List<AttributedExpression>();
  public Specification<Dafny.FrameExpression, FrameExpression> Reads { get; set; }

  private Function(Dafny.Function functionDafny) {
    Name = functionDafny.Name;
    Body = Expression.FromDafny(functionDafny.Body);
    _Decreases = Specification<Dafny.Expression, Expression>.FromDafny(functionDafny.Decreases);
    Ins.AddRange(functionDafny.Formals.Select(Formal.FromDafny));
    Out = functionDafny.Result == null ? null : Formal.FromDafny(functionDafny.Result);
    OutType = Type.FromDafny(functionDafny.ResultType);
    Req.AddRange(functionDafny.Req.Select(AttributedExpression.FromDafny));
    Ens.AddRange(functionDafny.Ens.Select(AttributedExpression.FromDafny));
    Reads = Specification<Dafny.FrameExpression, FrameExpression>.FromDafny(functionDafny.Reads);
  }

  public static Function FromDafny(Dafny.Function dafnyNode) {
    return new Function(dafnyNode);
  }
}