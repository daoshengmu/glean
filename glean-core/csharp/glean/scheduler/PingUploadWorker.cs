
using Glean.Net;
using System;
using static Glean.Net.FfiPingUploadTask;
using static Glean.Glean;

namespace Glean.Scheduler
{
  internal class PingUploadWorker
  {
    enum Result : byte
    {
      Success = 0,
      Retry = 1,
      Error = 2,
    }

    // Quene tasks 
    public static void Process()
    {
      if (Dispatcher.IsTestingMode())
      {
        _TestProcessSync();
      }
      else
      {
        _Process();
      }
    }

    private static void _TestProcessSync()
    {

    }

    private static void _Process()
    {

    }

    private static Result _DoWork()
    {
      do
      {
        var incomingTask = new FfiPingUploadTask();
        byte logPings = GleanInstance.configuration.logPings == true ? (byte)1 : (byte)0;
        Ffi.GleanGetUploadTask(ref incomingTask, logPings);

        var action = incomingTask.ToPingUploadTask();
        if (action.GetType() == typeof(PingUploadTaskUpload))
        {
          var uploadAction = (PingUploadTaskUpload)action;
          var result = GleanInstance.pingUploader.DoUpload(
                        uploadAction.request.path,
                        uploadAction.request.body,
                        uploadAction.request.headers,
                        GleanInstance.configuration);

        } else if (action.GetType() == typeof(PingUploadTaskWait))
        {
          return Result.Retry;
        } else if (action.GetType() == typeof(PingUploadTaskDone))
        {
          return Result.Success;
        } else
        {
          Console.Error.WriteLine("Undefined type of FfiPingUploadTask.");
          return Result.Error;
        }

      } while (true);
    }
  }
}
