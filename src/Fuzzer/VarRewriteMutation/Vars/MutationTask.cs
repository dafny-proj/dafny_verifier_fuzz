using System.Diagnostics.Contracts;

namespace Fuzzer;

public class TaskManager {
  private List<Task> Pending = new();
  private List<Task> Completed = new();

  public void CompleteTasks() {
    List<Task> Ready = new(Pending);
    Ready.ForEach(t => {
      t.Complete();
      Pending.Remove(t);
      Completed.Add(t);
    });
  }

  public void AddTask(Task task) {
    Contract.Requires(!task.IsCompleted);
    Pending.Add(task);
  }
}

public abstract class Task {
  public bool IsCompleted { get; private set; }

  public Task() {
    IsCompleted = false;
  }

  protected abstract void Action();

  public void Complete() {
    Action();
    IsCompleted = true;
  }
}