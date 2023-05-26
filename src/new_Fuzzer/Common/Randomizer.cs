namespace Fuzzer_new;

public interface IRandomizer {
  public int RandInt();
  public int RandInt(int maxValue);
  public int RandInt(int minValue, int maxValue);
  public bool RandBool();
  public IEnumerable<T> Shuffle<T>(IEnumerable<T> ts);
}
public class Randomizer : IRandomizer {
  private Random rand = new Random();

  public int RandInt() {
    return rand.Next();
  }
  public int RandInt(int maxValue) {
    return rand.Next(maxValue);
  }
  public int RandInt(int minValue, int maxValue) {
    return rand.Next(minValue, maxValue);
  }
  public bool RandBool() {
    return rand.NextDouble() < 0.5;
  }
  public IEnumerable<T> Shuffle<T>(IEnumerable<T> ts) {
    return ts.OrderBy(_ => rand.Next());
  }
  public T RandElement<T>(IEnumerable<T> ts) {
    Contract.Requires(ts.Count() > 0);
    return ts.ElementAt(rand.Next(maxValue: ts.Count()));
  }
}
