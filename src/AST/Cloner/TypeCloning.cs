namespace AST.Cloner;

public partial class ASTCloner {
  private Type CloneType(Type t) {
    return t switch {
      BasicType => t,
      NatType => t,
      StringType => t,
      MapType mt => new MapType(CloneType(mt.KeyType), CloneType(mt.ValueType)),
      SeqType st => new SeqType(CloneType(st.ElementType)),
      SetType st => new SetType(CloneType(st.ElementType)),
      MultiSetType mt => new MultiSetType(CloneType(mt.ElementType)),
      NullableType nt => new NullableType(
        classDecl: (ClassDecl)CloneDeclRef(nt.TypeDecl),
        typeArgs: nt.GetTypeArgs().Select(CloneType)),
      ArrowType at => new ArrowType(arrow: at.Arrow,
        typeDecl: (TopLevelDecl)CloneDeclRef(at.TypeDecl),
        typeArgs: at.GetTypeArgs().Select(CloneType)),
      UserDefinedType ut => new UserDefinedType(
        typeDecl: (TopLevelDecl)CloneDeclRef(ut.TypeDecl),
        typeArgs: ut.GetTypeArgs().Select(CloneType)),
      CallableType ct 
        => new CallableType((MemberDecl)CloneDeclRef(ct.Callable)),
      TypeProxy tt => new TypeProxy(),
      _ => throw new UnsupportedNodeCloningException(t),
    };
  }

}
