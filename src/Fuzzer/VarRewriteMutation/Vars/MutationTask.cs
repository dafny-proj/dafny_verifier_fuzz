using System.Diagnostics.Contracts;

namespace Fuzzer;

public class TaskManager {
  private List<Task> Pending = new();
  private List<Task> Completed = new();

  public void CompleteTasks() {
    while (Pending.Count > 0) {
      var Ready = Pending.Where(t => t.IsReady).ToList();
      if (Ready.Count == 0) {
        throw new Exception("All tasks are blocked.");
      }
      Ready.ForEach(t => {
        t.Complete();
        Pending.Remove(t);
        Completed.Add(t);
      });
    }
  }

  public void AddTask(Task task) {
    Contract.Requires(!task.IsCompleted);
    Pending.Add(task);
  }
}

public abstract class Task {
  public bool IsReady => BlockingDependencies.Count == 0;
  public bool IsCompleted { get; private set; }
  private List<Task> BlockingDependencies = new();
  private List<Task> CompletedDependencies = new();
  private List<Task> Successors = new();

  protected abstract void Action();

  public void Complete() {
    Action();
    IsCompleted = true;
    NotifySuccessorsOfCompletion();
  }

  public void AddBlockingDependency(IEnumerable<Task> dependencies) {
    foreach (var d in dependencies) {
      AddBlockingDependency(d);
    }
  }

  public void AddBlockingDependency(Task dependency) {
    Contract.Requires(!dependency.IsCompleted);
    BlockingDependencies.Add(dependency);
    dependency.Successors.Add(this);
  }

  private void NotifySuccessorsOfCompletion() {
    Successors.ForEach(s => s.MarkDependencyComplete(this));
  }

  private void MarkDependencyComplete(Task dependency) {
    Contract.Requires(dependency.IsCompleted && BlockingDependencies.Contains(dependency));
    BlockingDependencies.Remove(dependency);
    CompletedDependencies.Add(dependency);
  }
}