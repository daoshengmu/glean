
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Glean.Metrics;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Glean
{
  public class Glean {
    private static bool _initialized = false;
    private static bool _destroy_data_dir = false;
    private static Configuration _configuration;
    private static string _applicationId;
    private static string _applicationVersion;
    private static bool _uploadEnabled = false;
    private static List<PingType> _ping_type_queue = new List<PingType>();

    public static void Initialize(string aApplicationId, string aApplicationVersion,
      bool aUploadEnabled, Configuration aConfiguration = null, string aDataDir = null) {
      if (aConfiguration == null) {
        aConfiguration = new Configuration();
      }

      if (aDataDir == null) {
        aDataDir = Path.GetTempFileName();
        _destroy_data_dir = true;
      } else {
        _destroy_data_dir = false;
      }

      _configuration = aConfiguration;
      _applicationId = aApplicationId;
      _applicationVersion = aApplicationVersion;
      _uploadEnabled = aUploadEnabled;
        
      Ffi.FfiConfiguration cfg = Ffi.MakeConfig(aDataDir, aApplicationId, aUploadEnabled, 500 /**/);

      _initialized = Ffi.GleanInitialize(cfg) != 0;

      Console.WriteLine("Glean initialize: " + _initialized);

      if (!_initialized)
      {
        Console.WriteLine("Glean initialization is failed");
        return;
      }

      foreach(PingType ping in _ping_type_queue)
      {
        RegisterPingType(ping);
      }

      _InitializeCoreMetrics();
    }
   
    public static void RegisterPingType(PingType aPing)
    {
      if (_initialized)
      {
        Ffi.RegisterPingType(aPing.Handle());
      }
    }

    private static void _InitializeCoreMetrics()
    {
      // Read metric.yaml
      try
      {
        string content = File.ReadAllText("metrics.yaml", Encoding.UTF8);
        var r = new StringReader(content);
        var deserializer = new DeserializerBuilder()
               .Build();
        //var order = deserializer.Deserialize<metrics>(r);

      } catch(FileNotFoundException)
      {
        Console.Error.Write("Load metrics.yaml failed.");
      }
     
    }
  }
}