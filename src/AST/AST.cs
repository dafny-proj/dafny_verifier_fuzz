using Dafny = Microsoft.Dafny;

namespace AST;

public interface ConstructableFromDafny<S, T>
{
  static abstract T FromDafny(S dafnyNode);
  static System.Type DafnyType => typeof(S);
}

public abstract class Node : ConstructableFromDafny<Dafny.Node, Node>
{
  public static Node FromDafny(Dafny.Node dafnyNode)
  {
    return dafnyNode switch
    {
      Dafny.Program program => Program.FromDafny(program),
      Dafny.Declaration decl => Declaration.FromDafny(decl),
      Dafny.ModuleDefinition moduleDef => ModuleDefinition.FromDafny(moduleDef),
      Dafny.Statement stmt => Statement.FromDafny(stmt),
      Dafny.Expression expr => Expression.FromDafny(expr),
      Dafny.Formal formal => Formal.FromDafny(formal),
      Dafny.Type type => Type.FromDafny(type),
      Dafny.AssignmentRhs assignRhs => AssignmentRhs.FromDafny(assignRhs),
      // FIXME: Dafny.Specification<?> => ?
      _ => throw new NotImplementedException($"{dafnyNode.GetType()}"),
    };
  }
}

public class Program : Node, ConstructableFromDafny<Dafny.Program, Program>
{

  public ModuleDecl DefaultModule { get; set; }
  public ModuleDefinition DefaultModuleDef
  {
    get
    {
      return ((LiteralModuleDecl)DefaultModule).ModuleDef;
    }
  }

  private Program(Dafny.Program programDafny)
  {
    DefaultModule = ModuleDecl.FromDafny(programDafny.DefaultModule);
  }

  public static Program FromDafny(Dafny.Program dafnyNode)
  {
    return new Program(dafnyNode);
  }
}

public class Declaration
: Node, ConstructableFromDafny<Dafny.Declaration, Declaration>
{
  public static Declaration FromDafny(Dafny.Declaration dafnyNode)
  {
    return dafnyNode switch
    {
      Dafny.TopLevelDecl topLevelDecl
        => TopLevelDecl.FromDafny(topLevelDecl),
      _ => throw new NotImplementedException(),
    };
  }
}

public class MemberDecl
: Declaration, ConstructableFromDafny<Dafny.MemberDecl, MemberDecl>
{
  public static MemberDecl FromDafny(Dafny.MemberDecl dafnyNode)
  {
    return dafnyNode switch
    {
      Dafny.Method method => Method.FromDafny(method),
      _ => throw new NotImplementedException(),
    };

  }
}

public class Method
: MemberDecl, ConstructableFromDafny<Dafny.Method, Method>
{
  // TODO: TypeArgs, Req, Ens, Mod
  public string Name { get; set; }
  public BlockStmt Body { get; set; }
  public Specification<Dafny.Expression, Expression> Decreases { get; set; }
  public List<Formal> Ins = new List<Formal>();
  public List<Formal> Outs = new List<Formal>();

  private Method(Dafny.Method methodDafny)
  {
    Name = methodDafny.Name;
		Body = BlockStmt.FromDafny(methodDafny.Body);
    Decreases = Specification<Dafny.Expression, Expression>.FromDafny(methodDafny.Decreases);
    Ins.AddRange(methodDafny.Ins.Select(Formal.FromDafny));
    Outs.AddRange(methodDafny.Outs.Select(Formal.FromDafny));
  }
  public static Method FromDafny(Dafny.Method dafnyNode)
  {
    return new Method(dafnyNode);
  }
}

public class TopLevelDecl
: Declaration, ConstructableFromDafny<Dafny.TopLevelDecl, TopLevelDecl>
{
  public static TopLevelDecl FromDafny(Dafny.TopLevelDecl dafnyNode)
  {
    return dafnyNode switch
    {
      Dafny.ModuleDecl moduleDecl
        => ModuleDecl.FromDafny(moduleDecl),
      Dafny.ClassDecl classDecl
        => ClassDecl.FromDafny(classDecl),
      _ => throw new NotImplementedException(),
    };
  }
}

public class ModuleDecl
: TopLevelDecl, ConstructableFromDafny<Dafny.ModuleDecl, ModuleDecl>
{
  public static ModuleDecl FromDafny(Dafny.ModuleDecl dafnyNode)
  {
    return dafnyNode switch
    {
      Dafny.LiteralModuleDecl litModuleDecl
        => LiteralModuleDecl.FromDafny(litModuleDecl),
      _ => throw new NotImplementedException(),
    };
  }
}

public class LiteralModuleDecl
: ModuleDecl, ConstructableFromDafny<Dafny.LiteralModuleDecl, LiteralModuleDecl>
{
  public ModuleDefinition ModuleDef { get; set; }

  private LiteralModuleDecl(Dafny.LiteralModuleDecl moduleDeclDafny)
  {
    ModuleDef = ModuleDefinition.FromDafny(moduleDeclDafny.ModuleDef);
  }

  public static LiteralModuleDecl FromDafny(Dafny.LiteralModuleDecl dafnyNode)
  {
    return new LiteralModuleDecl(dafnyNode);
  }
}

public class ClassDecl
: TopLevelDecl, ConstructableFromDafny<Dafny.ClassDecl, ClassDecl>
{
  public List<MemberDecl> Members = new List<MemberDecl>();
  public readonly bool IsDefaultClass = false;

  private ClassDecl(Dafny.ClassDecl classDeclDafny)
  {
    Members.AddRange(classDeclDafny.Members.Select(MemberDecl.FromDafny));
    IsDefaultClass = classDeclDafny.IsDefaultClass;
  }

  public static ClassDecl FromDafny(Dafny.ClassDecl dafnyNode)
  {
    return new ClassDecl(dafnyNode);
  }
}

public class ModuleDefinition
: Node, ConstructableFromDafny<Dafny.ModuleDefinition, ModuleDefinition>
{
  public List<TopLevelDecl> TopLevelDecls = new List<TopLevelDecl>();

  private ModuleDefinition(Dafny.ModuleDefinition moduleDefDafny)
  {
    TopLevelDecls
      = moduleDefDafny.TopLevelDecls.Select(TopLevelDecl.FromDafny).ToList();
  }

  public static ModuleDefinition FromDafny(Dafny.ModuleDefinition dafnyNode)
  {
    return new ModuleDefinition(dafnyNode);
  }
}

public class Statement
: Node, ConstructableFromDafny<Dafny.Statement, Statement>
{
  public static Statement FromDafny(Dafny.Statement dafnyNode)
  {
    return dafnyNode switch
    {
      Dafny.BlockStmt blockStmt
        => BlockStmt.FromDafny(blockStmt),
      Dafny.UpdateStmt updateStmt
        => UpdateStmt.FromDafny(updateStmt),
      _ => throw new NotImplementedException(),
    };
  }
}

public class BlockStmt
: Statement, ConstructableFromDafny<Dafny.BlockStmt, BlockStmt>
{
  public List<Statement> Body = new List<Statement>();
  private BlockStmt(Dafny.BlockStmt blockStmtDafny)
  {
    Body.AddRange(blockStmtDafny.Body.Select(Statement.FromDafny));
  }
  public static BlockStmt FromDafny(Dafny.BlockStmt dafnyNode)
  {
    return new BlockStmt(dafnyNode);
  }
}

public class UpdateStmt
: Statement, ConstructableFromDafny<Dafny.UpdateStmt, UpdateStmt>
{
  public List<Expression> Lhss = new List<Expression>();
  public List<AssignmentRhs> Rhss = new List<AssignmentRhs>();

  private UpdateStmt(Dafny.UpdateStmt updateStmtDafny)
  {
    Lhss.AddRange(updateStmtDafny.Lhss.Select(Expression.FromDafny));
    Rhss.AddRange(updateStmtDafny.Rhss.Select(AssignmentRhs.FromDafny));
  }
  public static UpdateStmt FromDafny(Dafny.UpdateStmt dafnyNode)
  {
    return new UpdateStmt(dafnyNode);
  }
}

public abstract class Expression
: Node, ConstructableFromDafny<Dafny.Expression, Expression>
{
  public static Expression FromDafny(Dafny.Expression dafnyNode)
  {
    return dafnyNode switch
    {
      Dafny.NameSegment nameSeg
        => NameSegment.FromDafny(nameSeg),
      Dafny.BinaryExpr binExpr
        => BinaryExpr.FromDafny(binExpr),
      _ => throw new NotImplementedException(),
    };
  }
}

public class NameSegment
: Expression, ConstructableFromDafny<Dafny.NameSegment, NameSegment>
{
  public string Name { get; set; }
  private NameSegment(Dafny.NameSegment nameSegmentDafny)
  {
    Name = nameSegmentDafny.Name;
  }
  public static NameSegment FromDafny(Dafny.NameSegment dafnyNode)
  {
    return new NameSegment(dafnyNode);
  }
}

public class BinaryExpr
: Expression, ConstructableFromDafny<Dafny.BinaryExpr, BinaryExpr>
{
  public enum Opcode
  {
    Add,
  };
  public static Opcode FromDafny(Dafny.BinaryExpr.Opcode opDafny)
  {
    return opDafny switch
    {
      Dafny.BinaryExpr.Opcode.Add => Opcode.Add,
      _ => throw new NotImplementedException(),
    };
  }

	public static string OpcodeString(Opcode op) {
		return op switch {
			Opcode.Add => "+",
			_ => throw new NotImplementedException(),
		};
	}

  public Opcode Op { get; set; }
  public Expression E0 { get; set; }
  public Expression E1 { get; set; }
  private BinaryExpr(Dafny.BinaryExpr binaryExprDafny)
  {
    Op = FromDafny(binaryExprDafny.Op);
    E0 = Expression.FromDafny(binaryExprDafny.E0);
    E1 = Expression.FromDafny(binaryExprDafny.E1);
  }
  public static BinaryExpr FromDafny(Dafny.BinaryExpr dafnyNode)
  {
    return new BinaryExpr(dafnyNode);
  }
}

public class Specification<S, T>
: Node, ConstructableFromDafny<Dafny.Specification<S>, Specification<S, T>>
where S : Dafny.Node where T : Node, ConstructableFromDafny<S, T>
{
  public List<T> Expressions = new List<T>();

  private Specification(Dafny.Specification<S> specDafny)
  {
    Expressions.AddRange(specDafny.Expressions.Select(e => (T)Node.FromDafny(e)));
  }

  public static Specification<S, T> FromDafny(Dafny.Specification<S> dafnyNode)
  {
    return new Specification<S, T>(dafnyNode);
  }
}

public class Formal : Node, ConstructableFromDafny<Dafny.Formal, Formal>
{
  public string Name { get; set; }
  public Type Type { get; set; }
  public Formal(Dafny.Formal formalDafny)
  {
    Name = formalDafny.Name;
    Type = Type.FromDafny(formalDafny.Type);
  }
  public static Formal FromDafny(Dafny.Formal dafnyNode)
  {
    return new Formal(dafnyNode);
  }
}

public abstract class Type : Node, ConstructableFromDafny<Dafny.Type, Type>
{
	public abstract string Name {get; }
  public static Type FromDafny(Dafny.Type dafnyNode)
  {
    return dafnyNode switch
    {
      Dafny.IntType intType => IntType.FromDafny(intType),
      _ => throw new NotImplementedException(),
    };
  }
}

public class IntType : Type, ConstructableFromDafny<Dafny.IntType, IntType>
{
  private IntType() { }

  public override string Name { get => "int"; }

  public static IntType FromDafny(Dafny.IntType dafnyNode)
  {
    return new IntType();
  }
}

public abstract class AssignmentRhs
: Node, ConstructableFromDafny<Dafny.AssignmentRhs, AssignmentRhs>
{
  public static AssignmentRhs FromDafny(Dafny.AssignmentRhs dafnyNode)
  {
    return dafnyNode switch
    {
      Dafny.ExprRhs exprRhsDafny => ExprRhs.FromDafny(exprRhsDafny),
      _ => throw new NotImplementedException(),
    };
  }
}

public class ExprRhs
: AssignmentRhs, ConstructableFromDafny<Dafny.ExprRhs, ExprRhs>
{
  public Expression Expr;
  private ExprRhs(Dafny.ExprRhs exprRhsDafny)
  {
    Expr = Expression.FromDafny(exprRhsDafny.Expr);
  }
  public static ExprRhs FromDafny(Dafny.ExprRhs dafnyNode)
  {
    return new ExprRhs(dafnyNode);
  }
}