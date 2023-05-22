namespace AST_new.Cloner;

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
      UserDefinedType ut => new UserDefinedType(
        typeDecl: (TopLevelDecl)CloneDeclRef(ut.TypeDecl),
        typeArgs: ut.GetTypeArgs().Select(CloneType)),
      TypeProxy tt => new TypeProxy(),
      _ => throw new UnsupportedNodeCloningException(t),
    };
  }

}
