using Glean.Metrics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Glean
{
  internal class Buildins
  {
    private static bool _inited = false;
    private static Buildins _instance = null;
    private static Dictionary<String, Dictionary<String, object>> _metricTree = null;
    private static Dictionary<String, PingType> _pingTree = null;

    private Buildins() { }

    internal static Buildins Instance()
    {
      if (!_inited)
      {
        _inited = true;

        _instance = new Buildins();
        _LoadMetrics();
        _LoadPings();
      }

      Debug.Assert(_instance != null);
      return _instance;
    }

    internal Dictionary<String, Dictionary<String, object>> MetricsTree()
    {
      Debug.Assert(_metricTree != null);
      return _metricTree;
    }

    private static void _LoadMetrics()
    {
      var resourceFolder = (
          new System.Uri(Assembly.GetExecutingAssembly().CodeBase)
      ).AbsolutePath;

      var lastIdx = resourceFolder.LastIndexOf('/');
      var folderPath = resourceFolder.Substring(0, lastIdx + 1) + "resource/";

      _metricTree =  Loader.LoadMetrics(new List<string>()
        {folderPath + "metrics.yaml"}, new Dictionary<string, object>
        {{"allow_reserved", true}}
       );
    }

    private static void _LoadPings()
    {
      var resourceFolder = (
         new System.Uri(Assembly.GetExecutingAssembly().CodeBase)
     ).AbsolutePath;

      var lastIdx = resourceFolder.LastIndexOf('/');
      var folderPath = resourceFolder.Substring(0, lastIdx + 1) + "resource/";

      _pingTree = Loader.LoadPings(new List<string>()
        {folderPath + "pings.yaml"}, new Dictionary<string, object>
        {{"allow_reserved", true}}
       );
    }
  }
}
