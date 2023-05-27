namespace Fuzzer;

public interface IGenerator {
  public string GenLabelName();
  public string GenVarName();
  public string GenClassName();
}

public class BasicGenerator : IGenerator {
  private string id;
  private int count = 0;
  public BasicGenerator(string id) {
    this.id = id;
  }

  private string GenName(string prefix) => $"{prefix}{count++}_{id}";
  public string GenClassName() => GenName("C");
  public string GenLabelName() => GenName("l");
  public string GenVarName() => GenName("v");
}
