namespace Fuzzer.Tests;

public class MockRandomizer : Randomizer { }

public class MockGenerator : IGenerator {
  private int count = 0;

  public string GenClassName() {
    return $"C{count++}_mock";
  }

  public string GenLabelName() {
    return $"l{count++}_mock";
  }

  public string GenVarName() {
    return $"v{count++}_mock";
  }
}