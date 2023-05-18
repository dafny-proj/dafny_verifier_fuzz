namespace AST_new;

public partial class StringType : BuiltInType {
  public override string BaseName => "string";

  public static StringType Instance => new StringType();

  private StringType() { }
}