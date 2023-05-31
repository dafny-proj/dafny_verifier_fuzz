namespace AST;

public abstract class Node {
  public abstract IEnumerable<Node> Children { get; }
}

public partial class Program : Node { }

public abstract partial class Declaration : Node { }
public abstract partial class TopLevelDecl : Declaration { }
public partial class ModuleDecl : TopLevelDecl { }
public partial class ClassDecl : TopLevelDecl { }
public partial class TypeParameterDecl : TopLevelDecl { }
public partial class TypeSynonymDecl : TopLevelDecl { }
public partial class SubsetTypeDecl : TopLevelDecl { }
public partial class DatatypeDecl : TopLevelDecl { }
public partial class TupleTypeDecl : DatatypeDecl { }
public abstract partial class MemberDecl : Declaration { }
public partial class MethodDecl : MemberDecl { }
public partial class FunctionDecl : MemberDecl { }
public partial class FieldDecl : MemberDecl { }

public abstract partial class Statement : Node { }
public partial class BlockStmt : Statement { }
public partial class VarDeclStmt : Statement { }
public abstract partial class UpdateStmt : Statement { }
public partial class AssignStmt : UpdateStmt { }
public partial class CallStmt : UpdateStmt { }
public partial class PrintStmt : Statement { }
public partial class ReturnStmt : Statement { }
public partial class IfStmt : Statement { }
public abstract partial class LoopStmt : Statement { }
public partial class WhileLoopStmt : LoopStmt { }
public partial class ForLoopStmt : LoopStmt { }
public partial class BreakStmt : Statement { }
public partial class ContinueStmt : BreakStmt { }
public partial class AssertStmt : Statement { }
public partial class MatchStmt : Statement { }

public abstract partial class Expression : Node { }
public abstract partial class LiteralExpr : Expression { }
public partial class IdentifierExpr : Expression { }
public partial class ParensExpr : Expression { }
public partial class BinaryExpr : Expression { }
public partial class UnaryExpr : Expression { }
public partial class MemberSelectExpr : Expression { }
public abstract partial class CollectionSelectExpr : Expression { }
public abstract partial class CollectionDisplayExpr<T> : Expression where T : Node { }
public abstract partial class CollectionComprehensionExpr : Expression { }
public partial class CollectionUpdateExpr : Expression { }
public partial class MultiSetConstructionExpr : Expression { }
public partial class SeqConstructionExpr : Expression { }
public partial class DatatypeValueExpr : Expression { }
public partial class ThisExpr : Expression { }
public partial class AutoGeneratedExpr : Expression { }
public partial class WildcardExpr : Expression { }
public partial class FunctionCallExpr : Expression { }
public partial class StaticReceiverExpr : Expression { }
public partial class ITEExpr : Expression { }
public partial class LetExpr : Expression { }
public abstract partial class QuantifierExpr : Expression { }
public partial class MatchExpr : Expression { }
public partial class LambdaExpr : Expression { }
public partial class DatatypeUpdateExpr : Expression { }

public abstract partial class AssignmentRhs : Node { }
public partial class ExprRhs : AssignmentRhs { }
public partial class MethodCallRhs : AssignmentRhs { }
public partial class NewArrayRhs : AssignmentRhs { }
public partial class NewObjectRhs : AssignmentRhs { }

public abstract partial class Type : Node { }
public abstract partial class BasicType : Type { }
public partial class BoolType : BasicType { }
public partial class CharType : BasicType { }
public partial class IntType : BasicType { }
public partial class RealType : BasicType { }
// public partial class OrdinalType : BasicType { }
// public partial class BitVectorType : BasicType { }
public abstract partial class BuiltInType : UserDefinedType { }
public partial class NatType : BuiltInType { }
public partial class StringType : BuiltInType { }
public partial class ArrayType : BuiltInType { }
public abstract partial class CollectionType : Type { }
public partial class MapType : CollectionType { }
public partial class SeqType : CollectionType { }
public partial class SetType : CollectionType { }
public partial class MultiSetType : CollectionType { }
public partial class UserDefinedType : Type { }
public partial class TypeProxy : Type { }
public partial class NullableType : UserDefinedType { }
public partial class ArrowType : UserDefinedType { }

public abstract partial class Variable : Node { }
public partial class Specification : Node { }
public partial class KeyValuePair<K, V> : Node where K : Node where V : Node { }
public partial class QuantifierDomain : Node { }
public abstract partial class Matcher : Node { }
