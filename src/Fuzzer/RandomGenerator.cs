namespace Fuzzer;

public class RandomGenerator {
  private Random Generator = new Random();
  public bool Bool() {
    return Generator.NextDouble() < 0.5;
  }
}