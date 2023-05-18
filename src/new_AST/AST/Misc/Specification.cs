namespace AST_new;

// In more advanced cases, additional data specific to each specification type 
// may be present. For now, ignore the differences.
public partial class Specification {
  public Specification.Type SpecificationType { get; }
  public List<Expression> Expressions = new();

  public enum Type {
    Precondition, Postcondition, Invariant,
    Decreases, ReadFrame, ModifiesFrame,
  }
  public static string SpecificationTypeAsString(Specification.Type st) {
    return st switch {
      Type.Precondition => "requires",
      Type.Postcondition => "ensures",
      Type.Invariant => "invariant",
      Type.Decreases => "decreases",
      Type.ReadFrame => "reads",
      Type.ModifiesFrame => "modifies",
      _ => throw new NotSupportedException(
        $"Unsupported string conversion for `{st}`."),
    };
  }

  public Specification(Specification.Type specificationType,
  IEnumerable<Expression> expressions) {
    SpecificationType = specificationType;
    Expressions.AddRange(expressions);
  }

}