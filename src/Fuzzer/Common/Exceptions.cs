namespace Fuzzer;

public class UnsupportedMutationException : Exception {
  public UnsupportedMutationException(string? message = null)
  : base(message) { }
}
