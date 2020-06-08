using System;
using System.Collections.Generic;

namespace Glean
{
  internal class Dispatcher
  {
    // This value was chosen in order to allow several tasks to be queued for
    // execution but still be conservative of memory. This queue size is
    // important for cases where setUploadEnabled(false) is not called so that
    // we don't continue to queue tasks and waste memory.
    internal static int MAX_QUEUE_SIZE = 100;

    private static bool _testingMode = false;
    private static bool _queueInitialTasks = true;
    private static int _overflowCount = 0;
    private static List<Tuple<TaskDelelgate, List<string>, Dictionary<string, string>>>
      _preinitTaskQueue = new List<Tuple<TaskDelelgate, List<string>, Dictionary<string, string>>>();

    internal delegate void TaskDelelgate(List<string> aArg, Dictionary<string, string> aKwargs);

    internal static void Launch(Action aFunc)
    {
      aFunc();
    }

    internal static void LaunchAtFront(Action aFunc)
    {
      aFunc();
    }

    internal static bool IsTestingMode()
    {
      return _testingMode;
    }

    internal static void AssertInTestingMode()
    {

    }

    internal static void FlushQueuedInitialTasks()
    {
      // Stops queueing tasks and processes any tasks in the queue.
      SetTaskQueueing(false);

      foreach (var (task, args, kwargs) in _preinitTaskQueue)
      {
        // TODO: Implement _ExecuteTask
        _ExecuteTask(task, args, kwargs);
      }
      _preinitTaskQueue.Clear();

      if (_overflowCount > 0)
      {
        var metrics = Buildins.Instance().MetricsTree();
        if (metrics == null)
        {
          return;
        }
 
        try
        {
          ((glean_parser.Metric)(metrics["glean.error"]["preinit_tasks_overflow"])).Add(MAX_QUEUE_SIZE + _overflowCount);
        }
        catch(KeyNotFoundException)
        {
          Console.Error.WriteLine("'glean.error' doesn't have 'preinit_tasks_overflow' key from metric.");
        }
        _overflowCount = 0;
      }
    }

    internal static void SetTaskQueueing(bool aEnable)
    {
      _queueInitialTasks = aEnable;
    }

    private static void _ExecuteTask(TaskDelelgate func, List<string> args, Dictionary<string, string> kwargs)
    {
      // TODO:: thread worker to queue to run in another thread.
      // _task_worker.add_task(_testing_mode, func, args, **kwargs)
    }
  }
}
