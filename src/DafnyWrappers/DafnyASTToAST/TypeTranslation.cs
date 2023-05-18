using Dafny = Microsoft.Dafny;

namespace AST_new.Translation;

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
        => new SeqType(TranslateType(stt.Arg)),
      Dafny.MultiSetType mst
        => new SeqType(TranslateType(mst.Arg)),
      Dafny.UserDefinedType udt => TranslateUserDefinedType(udt),
      _ => throw new UnsupportedTranslationException(t),
    };
  }

  private Type TranslateUserDefinedType(Dafny.UserDefinedType udt) {
    if (udt.IsStringType) {
      return Type.String;
    } else if (udt.Name == "nat") {
      return Type.Nat;
    } else if (udt.IsArrayType) {
      var arrDecl = GetTranslatedDeclOrCreateSkeleton(udt.AsArrayType);
      return new ArrayType((ArrayClassDecl)arrDecl, TranslateType(udt.TypeArgs[0]));
    } else {
      return new UserDefinedType(TranslateTopLevelDecl(udt.ResolvedClass),
        udt.TypeArgs.Select(TranslateType));
    }
  }

}
