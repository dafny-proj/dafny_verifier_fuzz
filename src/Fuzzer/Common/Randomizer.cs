namespace Fuzzer;

public interface IRandomizer {
  public int RandInt();
  public int RandInt(int maxValue);
  public int RandInt(int minValue, int maxValue);
  public bool RandBool();
  public IEnumerable<T> Shuffle<T>(IEnumerable<T> ts);
  public IEnumerable<T> ShuffledSubset<T>(IEnumerable<T> ts);
  public T RandElement<T>(IEnumerable<T> ts);
}
public class Randomizer : IRandomizer {
  private Random rand = new Random();

  public virtual int RandInt() {
    return rand.Next();
  }
  public virtual int RandInt(int maxValue) {
    return rand.Next(maxValue);
  }
  public virtual int RandInt(int minValue, int maxValue) {
    return rand.Next(minValue, maxValue);
  }
  public virtual bool RandBool() {
    return rand.NextDouble() < 0.5;
  }
  public virtual IEnumerable<T> Shuffle<T>(IEnumerable<T> ts) {
    return ts.OrderBy(_ => rand.Next());
  }
  public virtual IEnumerable<T> ShuffledSubset<T>(IEnumerable<T> ts) {
    var numToRemove = RandInt(minValue: 0, maxValue: ts.Count());
    return Shuffle<T>(ts).Take(ts.Count() - numToRemove);
  }
  public virtual T RandElement<T>(IEnumerable<T> ts) {
    Contract.Requires(ts.Count() > 0);
    return ts.ElementAt(rand.Next(maxValue: ts.Count()));
  }
}
