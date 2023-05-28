using System.Diagnostics.Contracts;
using Dafny = Microsoft.Dafny;

namespace AST.Translation;

public partial class ASTTranslator {
  private AssignmentRhs TranslateAssignmentRhs(Dafny.AssignmentRhs ar) {
    return ar switch {
      Dafny.ExprRhs er => TranslateExprRhs(er),
      Dafny.TypeRhs tr => TranslateTypeRhs(tr),
      _ => throw new UnsupportedTranslationException(ar),
    };
  }

  private AssignmentRhs TranslateExprRhs(Dafny.ExprRhs er) {
    return new ExprRhs(TranslateExpression(er.Expr));
  }

  private AssignmentRhs TranslateTypeRhs(Dafny.TypeRhs tr) {
    if (tr.ArrayDimensions != null) {
      return TranslateNewArrayRhs(tr);
    } else {
      return TranslateNewObjectRhs(tr);
    }
  }

  private NewArrayRhs TranslateNewArrayRhs(Dafny.TypeRhs tr) {
    Contract.Requires(tr.ArrayDimensions != null);
    var elementType = TranslateType(tr.EType);
    var dimensions = tr.ArrayDimensions!.Select(TranslateExpression);
    if (tr.ElementInit != null) {
      return new NewArrayWithElementInitialiserRhs(elementType, dimensions,
        elementInitialiser: TranslateExpression(tr.ElementInit));
    } else if (tr.InitDisplay != null) {
      Contract.Assert(dimensions.Count() == 1);
      return new NewArrayWithListInitialiserRhs(elementType, dimensions.First(),
        listInitialiser: tr.InitDisplay.Select(TranslateExpression));
    } else {
      return new NewArrayRhs(elementType, dimensions);
    }
  }

  private NewObjectRhs TranslateNewObjectRhs(Dafny.TypeRhs tr) {
    var objectType = TranslateType(tr.EType);
    if (tr.InitCall != null) {
      return new NewObjectWithConstructorRhs(objectType,
        constructor: (ConstructorDecl)
          TranslateDeclRef((Dafny.Constructor)tr.InitCall.Method),
        constructorArguments: tr.InitCall.Args.Select(TranslateExpression));
    } else {
      return new NewObjectRhs(objectType);
    }
  }

}
