using System.Diagnostics.Contracts;

namespace AST;

public class MemberDecl
: Declaration, ConstructableFromDafny<Dafny.MemberDecl, MemberDecl> {
  public static MemberDecl FromDafny(Dafny.MemberDecl dafnyNode) {
    return dafnyNode switch {
      Dafny.Constructor constructor => Constructor.FromDafny(constructor),
      Dafny.Method method => Method.FromDafny(method),
      Dafny.Function func => Function.FromDafny(func),
      Dafny.DatatypeDestructor datatypeDestructor
        => DatatypeDestructor.FromDafny(datatypeDestructor),
      Dafny.DatatypeDiscriminator datatypeDiscriminator
        => DatatypeDiscriminator.FromDafny(datatypeDiscriminator),
      Dafny.Field field => Field.FromDafny(field),
      _ => throw new NotImplementedException(
          $"Unhandled translation from Dafny for `{dafnyNode.GetType()}`."
        ),
    };

  }
}

public class Method
: MemberDecl, ConstructableFromDafny<Dafny.Method, Method> {
  // TODO: TypeArgs
  public string? Name { get; set; }
  public bool IsAnonymous() => Name == null;
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

  public Method() {
    Body = BlockStmt.Empty();
    _Decreases = Specification<Dafny.Expression, Expression>.Empty();
    Mod = Specification<Dafny.FrameExpression, FrameExpression>.Empty();
  }

  protected Method(Dafny.Method methodDafny) {
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

  // TODO: do we include auto generated nodes in children?
  public override IEnumerable<Node> Children => new Node[] { }
      .Concat(Ins)
      .Concat(Outs)
      .Concat(Req)
      .Concat(Ens)
      .Append(AllDecreases)
      .Append(Mod)
      .Append(Body);
}

public class Constructor
: Method, ConstructableFromDafny<Dafny.Constructor, Constructor> {
  // TODO: Divided block stmt.
  public string Path { get; }

  public Constructor(ClassDecl cd, string? name = null) : base() {
    Name = name;
    Path = cd.Name ?? "";
    if (!this.IsAnonymous()) {
      Path += "." + Name;
    }
  }

  private Constructor(Dafny.Constructor cd) : base(cd) {
    Name = cd.HasName ? cd.Name : null;
    Path = cd.EnclosingClass.Name;
    if (!this.IsAnonymous()) {
      Path += "." + Name;
    }
  }

  public static Constructor FromDafny(Dafny.Constructor dafnyNode) {
    return new Constructor(dafnyNode);
  }
}

public class Function
: MemberDecl, ConstructableFromDafny<Dafny.Function, Function> {
  public override IEnumerable<Node> Children {
    get {
      var children = new Node[] { }.Concat(Ins);
      if (Out != null) {
        children.Append(Out);
      }
      children.Concat(Req)
        .Concat(Ens)
        .Append(AllDecreases)
        .Append(Reads)
        .Append(Body);
      return children;
    }
  }

  // TODO: TypeArgs, byMethodBody
  public string Name { get; set; }
  public Expression? Body { get; set; }
  private Specification<Dafny.Expression, Expression> _Decreases { get; set; }
  public Specification<Dafny.Expression, Expression> AllDecreases {
    get => _Decreases;
  }
  public Specification<Dafny.Expression, Expression> ProvidedDecreases {
    get => _Decreases.GetProvided();
  }
  public List<Formal> Ins = new List<Formal>();
  public Formal? Out { get; set; }
  // OutType is required as Out is not always set
  // e.g. function Foo(): int {} // Unnamed output variable => Out not set
  public Type OutType { get; set; }
  public List<AttributedExpression> Req = new List<AttributedExpression>();
  public List<AttributedExpression> Ens = new List<AttributedExpression>();
  public Specification<Dafny.FrameExpression, FrameExpression> Reads { get; set; }

  private Function(Dafny.Function fd) {
    Name = fd.Name;
    Body = fd.Body == null ? null : Expression.FromDafny(fd.Body);
    _Decreases = Specification<Dafny.Expression, Expression>.FromDafny(fd.Decreases);
    Ins.AddRange(fd.Formals.Select(Formal.FromDafny));
    Out = fd.Result == null ? null : Formal.FromDafny(fd.Result);
    OutType = Type.FromDafny(fd.ResultType);
    Req.AddRange(fd.Req.Select(AttributedExpression.FromDafny));
    Ens.AddRange(fd.Ens.Select(AttributedExpression.FromDafny));
    Reads = Specification<Dafny.FrameExpression, FrameExpression>.FromDafny(fd.Reads);
  }

  public static Function FromDafny(Dafny.Function dafnyNode) {
    return new Function(dafnyNode);
  }
}

public class Field : MemberDecl, ConstructableFromDafny<Dafny.Field, Field> {
  public string Name { get; }
  public Type Type { get; }

  public Field(string name, Type type) {
    Name = name;
    Type = type;
  }

  private Field(Dafny.Field fd) : this(fd.Name, Type.FromDafny(fd.Type)) { }

  public static Field FromDafny(Dafny.Field dafnyNode) {
    return new Field(dafnyNode);
  }
}