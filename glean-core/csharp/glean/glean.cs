
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using Glean.Scheduler;
using Glean.Metrics;
using Glean.Net;

namespace Glean
{
  public sealed class Glean {
    private bool _initialized = false;
    private string _dataDir = "";
    private bool _uploadEnabled = false;

    internal Configuration configuration;

    private bool _destroyDataDir;
    private string _applicationId;
    private string _applicationVersion;
    internal BaseUploader pingUploader;

    private List<PingType> _ping_type_queue = new List<PingType>();

    private Glean() { }

    // Initialize the singleton using the `Lazy` facilities.
    private static readonly Lazy<Glean>
      lazy = new Lazy<Glean>(() => new Glean());
    public static Glean GleanInstance => lazy.Value;


    public void Initialize(string aApplicationId, string aApplicationVersion,
      bool aUploadEnabled, Configuration aConfiguration = null, string aDataDir = null) {

      if (aConfiguration == null) {
        aConfiguration = new Configuration();
      }

      pingUploader = aConfiguration.pingUploader;
      configuration = aConfiguration;
      _applicationId = aApplicationId;
      _applicationVersion = aApplicationVersion;
      _uploadEnabled = aUploadEnabled;
        
      Ffi.FfiConfiguration cfg = Ffi.MakeConfig(aDataDir, aApplicationId, aUploadEnabled, Configuration.DEFAULT_MAX_EVENTS);

      _initialized = Ffi.GleanInitialize(cfg);

      Console.WriteLine("Glean initialize: " + _initialized);

      if (!_initialized)
      {
        Debug.Assert(false);
        Console.Error.WriteLine("Glean initialization is failed");
        return;
      }
      
      if (aDataDir == null)
      {
        aDataDir = Path.GetTempFileName();
        _destroyDataDir = true;
      }
      else
      {
        _destroyDataDir = false;
      }
      _dataDir = aDataDir;

      foreach (PingType ping in _ping_type_queue)
      {
        RegisterPingType(ping);
      }

      var isFirstRun = Ffi.GleanIsFirstRun();
      if (isFirstRun)
      {
        _InitializeCoreMetrics();
      } 
      
      var pingSubmitted = Ffi.GleanOnReadyToSubmitPings();
     
      // TODO: Implement PingUpload worker threads
      // We need to enqueue the PingUploadWorker in these cases:
      // 1. Pings were submitted through Glean and it is ready to upload those pings;
      // 2. Upload is disabled, to upload a possible deletion-request ping.
      if (pingSubmitted || !_uploadEnabled)
      {
        PingUploadWorker.Process();
      }

      if (!isFirstRun)
      {
        Ffi.GleanClearApplicationLifetimeMetrics();
        _InitializeCoreMetrics();
      }

      // TODO: implement flush queued intial tasks.
      Dispatcher.FlushQueuedInitialTasks();
    }

    internal void RegisterPingType(PingType aPing)
    {
      if (_initialized)
      {
        Ffi.RegisterPingType(aPing.Handle());
      }
    }

    public string GetDataDir()
    {
      return _dataDir;
    }

    public void SetUploadEnabled(bool aEnable)
    {
      Ffi.GleanSetUploadEnabled(aEnable ? (byte)1 : (byte)0);
    }

    public bool IsUploadEnabled()
    {
      return Ffi.GleanIsUploadEnabled() > 0;
    }

    internal void SubmitPing(PingType aPing, string aReason = null)
    {
      _SubmitPingsByName(aPing.Name(), aReason);
    }

    // TODO: support PingUploadWorker.Process();
    private void _SubmitPingsByName(string aPingName, string aReason = null)
    {
      if (_initialized == false) {
        Console.Error.WriteLine("Glean must be initialized before submitting pings.");
        return;
      }

      if (_uploadEnabled == false)
      {
        Console.Error.WriteLine("Glean disabled: not submitting any pings.");
        return;
      }

      var sentPing = Ffi.GleanSubmitPingByName(aPingName, aReason);

      if (sentPing == true)
      {
        PingUploadWorker.Process();
      }
    }

    private void _InitializeCoreMetrics()
    {
      CultureInfo ci = CultureInfo.InstalledUICulture;

      // Read metric.yaml
      try
      {
        var metrics = Buildins.Instance().MetricsTree();
        ((glean_parser.Metric)metrics["glean.baseline"]["locale"]).Set(ci.IetfLanguageTag);
        ((glean_parser.Metric)metrics["glean.internal.metrics"]["os_version"]).Set(Environment.OSVersion.Version);
        ((glean_parser.Metric)metrics["glean.internal.metrics"]["architecture"]).Set(RuntimeInformation.OSArchitecture.ToString().ToLower());
        ((glean_parser.Metric)metrics["glean.internal.metrics"]["locale"]).Set(ci.IetfLanguageTag);

        if (configuration != null)
        {
          ((glean_parser.Metric)metrics["glean.internal.metrics"]["app_channel"]).Set(configuration.channel);
        }

        ((glean_parser.Metric)metrics["glean.internal.metrics"]["app_build"]).Set(_applicationId);

        if (_applicationVersion != null)
        {
          ((glean_parser.Metric)metrics["glean.internal.metrics"]["app_display_version"]).Set(_applicationVersion);
        }
      } catch (KeyNotFoundException)
      {
        Console.Error.WriteLine("KeyNotFoundException in  metric.yaml.");
      }
    }
  }
}