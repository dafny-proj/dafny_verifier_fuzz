using Dafny = Microsoft.Dafny;

namespace AST;

public interface ConvertableWithDafnyAST<S, T>
{
  static abstract T FromDafnyAST(S DafnyNode);
  static abstract S ToDafnyAST(T Node);
}

public abstract class Node { }

public class Program : Node, ConvertableWithDafnyAST<Dafny.Program, Program>
{
  public static Program FromDafnyAST(Dafny.Program DafnyNode)
  {
    throw new NotImplementedException();
  }
  public static Dafny.Program ToDafnyAST(Program Node)
  {
    throw new NotImplementedException();
  }
}

public abstract class Expression
: Node, ConvertableWithDafnyAST<Dafny.Expression, Expression>
{
  public static Expression FromDafnyAST(Dafny.Expression DafnyNode)
  {
    throw new NotImplementedException();
  }
  public static Dafny.Expression ToDafnyAST(Expression Node)
  {
    throw new NotImplementedException();
  }
}

public abstract class Statement
: Node, ConvertableWithDafnyAST<Dafny.Statement, Statement>
{
  public static Statement FromDafnyAST(Dafny.Statement DafnyNode)
  {
    throw new NotImplementedException();
  }
  public static Dafny.Statement ToDafnyAST(Statement Node)
  {
    throw new NotImplementedException();
  }
}