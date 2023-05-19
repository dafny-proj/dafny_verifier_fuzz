namespace AST_new;

public abstract class Node {
  public virtual IEnumerable<Node> Children {
    get => throw new UnsupportedNodeChildrenException(this);
  }

  public virtual Node Clone() {
    throw new UnsupportedNodeCloningException(this);
  }

  public virtual Node ReplaceChild(Node oldChild, Node newChild) {
    throw new UnsupportedNodeChildReplacementException(this);
  }
}

public partial class Program : Node { }

public abstract partial class Declaration : Node { }
public abstract partial class TopLevelDecl : Declaration { }
public partial class ModuleDecl : TopLevelDecl { }
public partial class ClassDecl : TopLevelDecl { }
public partial class DefaultClassDecl : ClassDecl { }
public partial class ArrayClassDecl : ClassDecl { }
public partial class TypeParameter : TopLevelDecl { }
public partial class TypeSynonymDecl : TopLevelDecl { }
public partial class SubsetTypeDecl : TopLevelDecl { }
public abstract partial class MemberDecl : Declaration { }
public partial class MethodDecl : MemberDecl { }

public abstract partial class Statement : Node { }
public partial class BlockStmt : Statement { }
public partial class VarDeclStmt : Statement { }
public abstract partial class UpdateStmt : Statement { }
public partial class AssignStmt : UpdateStmt { }
public partial class CallStmt : UpdateStmt { }
public partial class PrintStmt : Statement { }
public partial class ReturnStmt : Statement { }

public abstract partial class Expression : Node { }
public abstract partial class LiteralExpr : Expression { }
public partial class IdentifierExpr : Expression { }
public partial class ParensExpr : Expression { }
public partial class BinaryExpr : Expression { }
public partial class UnaryExpr : Expression { }
public partial class MemberSelectExpr : Expression { }
public abstract partial class CollectionSelectExpr : Expression { }
public abstract partial class CollectionDisplayExpr<T> : Expression { }

public abstract partial class AssignmentRhs : Node { }
public partial class ExprRhs : AssignmentRhs { }
public partial class MethodCallRhs : AssignmentRhs { }

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

public abstract partial class Variable : Node { }
public partial class Specification : Node { }
public partial class KeyValuePair<K, V> : Node { }
