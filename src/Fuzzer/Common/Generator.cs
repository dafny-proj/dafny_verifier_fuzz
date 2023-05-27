namespace Fuzzer;

public interface IGenerator {
  public string GenLabelName();
  public string GenVarName();
  public string GenClassName();
}