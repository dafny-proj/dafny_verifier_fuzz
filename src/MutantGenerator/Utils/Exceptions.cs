namespace MutantGenerator;

public class UnsupportedMutationException : Exception {
  public UnsupportedMutationException(string? message = null)
  : base(message) { }
}
