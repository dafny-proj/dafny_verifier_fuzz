using System.Diagnostics;

namespace Microsoft.Dafny {

  public class FuzzMain {

    public static readonly string[] VerificationArgs = { "verify", "--cores=2", "--use-basename-for-filename", "--verification-time-limit=300" };

    public static (int, string, string) RunDafnyVerify(string filepath) {
      // Verification API is not easily callable, use Dafny CLI instead
      string dafnyCLIPath = "/Users/wyt/Desktop/dafny_proj/dafny/Scripts/dafny";
      var process = new Process();
      process.StartInfo.FileName = dafnyCLIPath;
      foreach (var vArg in VerificationArgs) {
        process.StartInfo.ArgumentList.Add(vArg);
      }
      process.StartInfo.ArgumentList.Add(filepath);
      process.StartInfo.UseShellExecute = false;
      process.StartInfo.RedirectStandardInput = true;
      process.StartInfo.RedirectStandardOutput = true;
      process.StartInfo.RedirectStandardError = true;
      process.Start();
      string output = process.StandardOutput.ReadToEnd();
      string error = process.StandardError.ReadToEnd();
      process.WaitForExit();
      return (process.ExitCode, output, error);
    }
    public static int MainOld(string[] args) {
      // For now, assume no errors while parsing command line or file
      string fileToVerify = "../../examples/Array.dfy";
      var cliArgumentsResult = DafnyDriver.ProcessCommandLineArguments(VerificationArgs.Append(fileToVerify).ToArray(), out var dafnyOptions, out var dafnyFiles, out var otherFiles);
      ErrorReporter reporter = new ConsoleErrorReporter(dafnyOptions);
      Program dafnyProgram;
      var (vCode, vOut, vErr) = RunDafnyVerify(fileToVerify);
      Console.WriteLine($"Verifying File: {fileToVerify}");
      Dafny.Main.Parse(dafnyFiles, fileToVerify, reporter, out dafnyProgram);
      Dafny.Main.MaybePrintProgram(dafnyProgram, "-", false);
      Console.WriteLine($"Exit Code: {vCode}");
      Console.WriteLine($"Output: {vOut}");
      Console.WriteLine($"Error: {(String.IsNullOrEmpty(vErr) ? "Empty" : vErr)}");
      return 0;
    }
  }

}

