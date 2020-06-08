
using System;
using System.Diagnostics;
using Glean;
using static Glean.Glean;

namespace demo
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("Hello World!");

      GleanInstance.Initialize(
        "fxrpc",
        "0.1.1",
        true,
        new Glean.Configuration(
        Glean.Configuration.DEFAULT_TELEMETRY_ENDPOINT, null, null, null, true,
        "fxrpc-test-tag"),
        "data"
      );

      GleanInstance.SetUploadEnabled(true);
      bool enabled = GleanInstance.IsUploadEnabled();

      // Using BuildIn.cs
      var metrics = Loader.LoadMetrics("metrics.yaml");
      Debug.Assert(metrics != null);
      var pings = Loader.LoadPings("pings.yaml");
      Debug.Assert(pings != null);

      ((Glean.glean_parser.Metric)metrics["usage"]["app"]).Set("reality");

      // TOOD: Conert parer.Ping to PingType
      pings["usage"].Submit(null);

      Console.WriteLine("Done,"); ;

    //  RunPythonProcess();
    }

    static void RunPythonProcess()
    {
      var psi = new ProcessStartInfo();
      psi.FileName = @"C:\\Program Files\WindowsApps\" +
        @"PythonSoftwareFoundation.Python.3.8_3.8.1008.0_x64__qbz5n2kfra8p0\python3.8.exe";
      var script = "helloWorld.py";

      psi.Arguments = $"\"{script}\"";
      psi.UseShellExecute = false;
      psi.CreateNoWindow = true;
      psi.RedirectStandardError = true;
      psi.RedirectStandardOutput = true;

      var errors = "";
      var results = "";

      using (var process = Process.Start(psi))
      {
        errors = process.StandardError.ReadToEnd();
        results = process.StandardOutput.ReadToEnd();
      }

      Console.WriteLine("Error:");
      Console.WriteLine(errors);
      Console.WriteLine("Result:");
      Console.WriteLine(results);
    }
  }
}

