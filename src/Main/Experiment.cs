namespace DafnyWrappers;

public class Experiment {
  public static int Main(string[] args) {
    var programDafny = DafnyWrappers.ParseDafnyProgramFromFile("../../examples/sum.dfy");
    var prog = AST.Program.FromDafny(programDafny);
    Console.WriteLine(prog);
    // var children = programDafny.Children;
    // int depth = 0;
    // while (children.Count() != 0) {
    //   Console.WriteLine(depth);
    //   var nextGen = new List<Microsoft.Dafny.Node>();
    //   foreach (var child in children) {
    //     Console.WriteLine(child.GetType());
    //     nextGen.AddRange(child.Children);
    //   }
    //   children = nextGen;
    //   depth++;
    // }
    return 0;
  }
}