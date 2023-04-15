namespace AST;

public class Declaration
: Node, ConstructableFromDafny<Dafny.Declaration, Declaration> {
  public static Declaration FromDafny(Dafny.Declaration dafnyNode) {
    return dafnyNode switch {
      Dafny.TopLevelDecl topLevelDecl
        => TopLevelDecl.FromDafny(topLevelDecl),
      _ => throw new NotImplementedException(),
    };
  }
}

public class TopLevelDecl
: Declaration, ConstructableFromDafny<Dafny.TopLevelDecl, TopLevelDecl> {
  public static TopLevelDecl FromDafny(Dafny.TopLevelDecl dafnyNode) {
    return dafnyNode switch {
      Dafny.ModuleDecl moduleDecl
        => ModuleDecl.FromDafny(moduleDecl),
      Dafny.ClassDecl classDecl
        => ClassDecl.FromDafny(classDecl),
      _ => throw new NotImplementedException(),
    };
  }
}

public class ModuleDecl
: TopLevelDecl, ConstructableFromDafny<Dafny.ModuleDecl, ModuleDecl> {
  public static ModuleDecl FromDafny(Dafny.ModuleDecl dafnyNode) {
    return dafnyNode switch {
      Dafny.LiteralModuleDecl litModuleDecl
        => LiteralModuleDecl.FromDafny(litModuleDecl),
      _ => throw new NotImplementedException(),
    };
  }
}

public class LiteralModuleDecl
: ModuleDecl, ConstructableFromDafny<Dafny.LiteralModuleDecl, LiteralModuleDecl> {
  public ModuleDefinition ModuleDef { get; set; }

  private LiteralModuleDecl(Dafny.LiteralModuleDecl moduleDeclDafny) {
    ModuleDef = ModuleDefinition.FromDafny(moduleDeclDafny.ModuleDef);
  }

  public static LiteralModuleDecl FromDafny(Dafny.LiteralModuleDecl dafnyNode) {
    return new LiteralModuleDecl(dafnyNode);
  }
}

public class ClassDecl
: TopLevelDecl, ConstructableFromDafny<Dafny.ClassDecl, ClassDecl> {
  public List<MemberDecl> Members = new List<MemberDecl>();
  public readonly bool IsDefaultClass = false;

  private ClassDecl(Dafny.ClassDecl classDeclDafny) {
    Members.AddRange(classDeclDafny.Members.Select(MemberDecl.FromDafny));
    IsDefaultClass = classDeclDafny.IsDefaultClass;
  }

  public static ClassDecl FromDafny(Dafny.ClassDecl dafnyNode) {
    return new ClassDecl(dafnyNode);
  }
}

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
  public Specification<Dafny.Expression, Expression> Decreases { get; set; }
  public List<Formal> Ins = new List<Formal>();
  public List<Formal> Outs = new List<Formal>();
  public List<AttributedExpression> Req = new List<AttributedExpression>();
  public List<AttributedExpression> Ens = new List<AttributedExpression>();
  public Specification<Dafny.FrameExpression, FrameExpression> Mod { get; set; }


  private Method(Dafny.Method methodDafny) {
    Name = methodDafny.Name;
    Body = BlockStmt.FromDafny(methodDafny.Body);
    Decreases = Specification<Dafny.Expression, Expression>.FromDafny(methodDafny.Decreases);
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
  public Specification<Dafny.Expression, Expression> Decreases { get; set; }
  public List<Formal> Ins = new List<Formal>();
  public Formal? Out { get; set; }
  public Type OutType { get; set; }
  public List<AttributedExpression> Req = new List<AttributedExpression>();
  public List<AttributedExpression> Ens = new List<AttributedExpression>();
  public Specification<Dafny.FrameExpression, FrameExpression> Reads { get; set; }

  private Function(Dafny.Function functionDafny) {
    Name = functionDafny.Name;
    Body = Expression.FromDafny(functionDafny.Body);
    Decreases = Specification<Dafny.Expression, Expression>.FromDafny(functionDafny.Decreases);
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