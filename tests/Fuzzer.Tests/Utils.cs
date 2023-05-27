namespace Fuzzer.Tests;

public class MockRandomizer : Randomizer { }

public class MockGenerator : BasicGenerator {
  public MockGenerator() : base("mock") { }
}