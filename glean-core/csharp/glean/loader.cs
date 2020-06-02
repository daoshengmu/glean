using System;
using System.Collections.Generic;
using Glean.glean_parser;
using Glean.Metrics;

namespace Glean
{
  using MetricType = Dictionary<string, Dictionary<string, object>>;
 // using PingType = Dictionary<string, object>;

  public class Loader
  {
    public static MetricType LoadMetrics(List<string> aFilePath,
        Dictionary<string, object> aConfig = null)
    {
      var result = Parser.ParseObjects(aFilePath, aConfig).tree;
      if (result.Count == 0)
      {
        Console.Error.WriteLine("Loading metrics.yaml failed.");
        return null;
      }
      return result;
    }

    public static MetricType LoadMetrics(string aFilePath,
        Dictionary<string, object> aConfig = null)
    {
      var fileList = new List<string> { aFilePath };
      return LoadMetrics(fileList);
    }

    internal static Dictionary<string, PingType> LoadPings(List<string> aFilePath, Dictionary<string, object> aDct = null)
    {
      //Load pings from a `pings.yaml` file.

      //Args:
      //    filepath(Path): The path to the file, or a list of paths, to load.
      //    config(dict): A dictionary of options that change parsing behavior.
      //        These are documented in glean_parser:
      //https://mozilla.github.io/glean_parser/glean_parser.html#glean_parser.parser.parse_objects
      //Returns:
      //  pings(object): An object where the attributes are pings, as defined in
      //        the `pings.yaml` file.
      //Example:
      //    >>> pings = load_pings("pings.yaml")
      //    >>> pings.baseline.submit()
      var metric = LoadMetrics(aFilePath, aDct);
      if (metric == null || !metric.ContainsKey("pings"))
      {
        Console.Error.WriteLine("Loading pings.yaml failed.");
        return null;
      }

      var pings = new Dictionary<string, PingType>();
      foreach (var pingData in metric["pings"])
      {
        var data = (Ping)pingData.Value;
        pings[pingData.Key] = new PingType(data.name,  data.includeClientId, data.sendIfEmpty, new List<string>());
      }
      return pings;
    }

    public static Dictionary<string, PingType> LoadPings(string aFilePath, Dictionary<string, object> aConfig = null)
    {
      var fileList = new List<string> { aFilePath };
      return LoadPings(fileList, aConfig);
    }
  }
}
