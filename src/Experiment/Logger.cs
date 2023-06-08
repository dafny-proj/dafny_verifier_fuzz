namespace Fuzzer;

public interface ILogger {
  public void LogCheckPoint(string msg);
  public void LogError(string msg);
  public void Close();
}

public abstract class ALogger : ILogger {
  public abstract void LogCheckPoint(string msg);
  public abstract void LogError(string msg);
  public virtual void Close() { }
}

public class ConsoleLogger : ALogger {
  private void Log(string msg) => Console.WriteLine(msg);
  public override void LogCheckPoint(string msg) => Log(msg);
  public override void LogError(string msg) => Log(msg);
}

public class SingleLogger : ALogger {
  private string seed;
  private string log;
  private StreamWriter? writer;

  public SingleLogger(string seed, string log) {
    this.seed = seed;
    this.log = log;
  }

  public void Log(string msg) {
    if (writer == null) {
      writer = new StreamWriter(log, append: true);
      writer.WriteLine($"==========================================");
      writer.WriteLine($"Logging for `{seed}`.");
    }
    writer.WriteLine(msg);
  }
  public override void LogCheckPoint(string msg) => Log(msg);
  public override void LogError(string msg) => Log("ERROR: " + msg);
  public override void Close() => writer?.Close();
}

public class DualLogger : ALogger {
  private string seed;
  private string checkPointLog;
  private string errorLog;
  private StreamWriter? checkPointWriter;
  private StreamWriter? errorWriter;

  public DualLogger(string seed, string checkPointLog, string errorLog) {
    this.seed = seed;
    this.checkPointLog = checkPointLog;
    this.errorLog = errorLog;
  }

  public override void LogCheckPoint(string msg) {
    if (checkPointWriter == null) {
      checkPointWriter = new StreamWriter(checkPointLog, append: true);
      checkPointWriter.WriteLine($"==========================================");
      checkPointWriter.WriteLine($"Logging for `{seed}`.");
    }
    checkPointWriter.WriteLine(msg);
  }
  public override void LogError(string msg) {
    if (errorWriter == null) {
      errorWriter = new StreamWriter(errorLog, append: true);
    }
    var errHeader = $"Error while processing {seed}.";
    LogCheckPoint(errHeader);
    errorWriter.WriteLine(errHeader);
    errorWriter.WriteLine(msg);
    errorWriter.WriteLine();
  }
  public override void Close() {
    checkPointWriter?.Close();
    errorWriter?.Close();
  }
}