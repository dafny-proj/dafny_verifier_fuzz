using Dafny = Microsoft.Dafny;

namespace AST.Translation;

public partial class DafnyASTTranslator {
  private Type TranslateType(Dafny.Type t) {
    return t switch {
      Dafny.BoolType => Type.Bool,
      Dafny.CharType => Type.Char,
      Dafny.IntType => Type.Int,
      Dafny.RealType => Type.Real,
      Dafny.MapType mt
        => new MapType(TranslateType(mt.Domain), TranslateType(mt.Range)),
      Dafny.SeqType sqt
        => new SeqType(TranslateType(sqt.Arg)),
      Dafny.SetType stt
        => new SetType(TranslateType(stt.Arg)),
      Dafny.MultiSetType mst
        => new MultiSetType(TranslateType(mst.Arg)),
      Dafny.UserDefinedType udt => TranslateUserDefinedType(udt),
      Dafny.TypeProxy tp => TranslateTypeProxy(tp),
      _ => throw new UnsupportedTranslationException(t),
    };
  }

  private Type TranslateUserDefinedType(Dafny.UserDefinedType udt) {
    if (udt.IsStringType) {
      return Type.String;
    } else if (udt.Name == "nat") {
      return Type.Nat;
    } else if (udt.IsArrayType) {
      var arrDecl = (ArrayClassDecl)TranslateDeclRef(udt.AsArrayType);
      return new ArrayType(arrDecl, TranslateType(udt.TypeArgs[0]));
    } else {
      return new UserDefinedType(
        (TopLevelDecl)TranslateDeclRef(udt.ResolvedClass),
        udt.TypeArgs.Select(TranslateType));
    }
  }

  private Type TranslateTypeProxy(Dafny.TypeProxy tp) {
    if (tp.T != null) {
      return TranslateType(tp.T);
    }
    return new TypeProxy();
  }

}
