namespace Fuzzer;

public interface IGenerator {
  public string GenLabelName();
  public string GenVarName();
  public string GenClassName();
  public string GenFunctionName();
  public string GenFormalName();
}

public class BasicGenerator : IGenerator {
  private string suffix;
  private int count = 0;
  public BasicGenerator(string suffix = "") {
    this.suffix = suffix;
  }
  public void Reset(string suffix = "") {
    this.count = 0;
    this.suffix = suffix;
  }

  private string GenName(string prefix) => $"{prefix}{count++}_{suffix}";
  public string GenClassName() => GenName("C");
  public string GenLabelName() => GenName("l");
  public string GenVarName() => GenName("v");
  public string GenFunctionName() => GenName("fn");
  public string GenFormalName() => GenName("fl");
}
