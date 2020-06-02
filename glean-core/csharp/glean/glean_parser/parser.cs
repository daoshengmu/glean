using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using StringPair = System.Collections.Generic.KeyValuePair<string, string>;

namespace Glean.glean_parser
{
  public class Parser
  {
    public const string METRICS_ID = "moz://mozilla.org/schemas/glean/metrics/1-0-0";
    public const string PINGS_ID = "moz://mozilla.org/schemas/glean/pings/1-0-0";
    public static Dictionary<string, string> FILE_TYPES = new Dictionary<string, string>{
      {METRICS_ID, "metrics"}, {PINGS_ID, "pings"}
    };

    public static object Mertics { get; private set; }

    public static Metric.ObjectTree ParseObjects(List<string> aFilePaths, Dictionary<string, object> aConfig)
    {
      var allObjects = new Metric.ObjectTree();
      foreach (string path in aFilePaths)
      {
        // Dictionary<string, string> content = new Dictionary<string, string>();
        //  Dictionary<string, object> fileType = new Dictionary<string, object>();
        YamlMappingNode content = null;
        string fileType = "";
        _loadFile(path, ref content, ref fileType);

        var source = new Dictionary<object, string>();
        if (fileType.Equals("metrics"))
        {
          _InstantiateMetrics(ref allObjects, source, content, path, aConfig);
        } else if (fileType.Equals("pings"))
        {
          _InstantiatePings(ref allObjects, source, content, path, aConfig);
        }
      }

      return allObjects;
    }

    private static void _InstantiateMetrics(ref Metric.ObjectTree aAllObjects,
                                            Dictionary<object, string> aSources,
                                            YamlMappingNode aContent,
                                            string aFilepath,
                                            Dictionary<string, object> aConfig)
    {
      var globalNoLint = new List<string>();

      if (aContent.Children.ContainsKey("no_lint"))
      {
        var noLints = (YamlSequenceNode)aContent.Children[new YamlScalarNode("no_lint")];
        foreach (var noLint in noLints)
        {
          globalNoLint.Add(((YamlScalarNode)noLint).Value);
        }
      }
     
      foreach (var category in aContent.Children)
      {
        if (category.Key.ToString()[0].Equals('$'))
        {
          continue;
        }

        if (category.Key.Equals("no_lint"))
        {
          continue;
        }

        bool? allow_reserved = false;
        if (aConfig != null && aConfig.ContainsKey("allow_reserved"))
        {
          allow_reserved = aConfig["allow_reserved"] as bool?;
        }
        if (allow_reserved != null && allow_reserved == false &&
          category.Key.ToString().Split()[0].Equals("glean"))
        {
          Console.WriteLine("{0}: For category '{1}', Categories beginning with 'glean'" +
               "are reserved for Glean internal use.", aFilepath, category.Key);
          continue;
        }

        aAllObjects.tree[category.Key.ToString()] = new Dictionary<string, object>();
        var categoryVal = category.Value;
        if (categoryVal.NodeType != YamlNodeType.Mapping)
        {
          throw new System.TypeLoadException(String.Format("Invalid content for category {0}", category.Key));
        }
        else
        {
          foreach (var metric in ((YamlMappingNode)categoryVal).Children)
          {
           // aAllObjects.tree[category.Key.ToString()] = metric.Key.ToString()].Set(metric.Key.ToString());
            var metricObj = Metric.MakeMetric(category.Key.ToString(), metric.Key.ToString(), (YamlMappingNode)metric.Value, aConfig, true);

            if (allow_reserved != null && allow_reserved == false &&
                metricObj.send_in_pings != null && metricObj.send_in_pings.IndexOf("all-pings") > 0)
            {
              Console.WriteLine("{0}: On instance {1}.{2} Only internal metrics may specify" +
                "'all - pings' in 'send_in_pings'", aFilepath, category.Key, metric.Key);
              metricObj = null;
            }

            if (metricObj != null && globalNoLint.Count > 0)
            {
              metricObj.noLint.AddRange(globalNoLint);
            }

            if (aSources.ContainsKey(new StringPair
             (category.Key.ToString(), metric.Key.ToString())))
            {
              string alreadySeen = aSources[new StringPair(category.Key.ToString(), metric.Key.ToString())];
              Console.WriteLine("{0}: Duplicate metric name '{1}.{2}' already defined in '{3}'",
                aFilepath, category.Key, metric.Key, alreadySeen);
            } else {
              aAllObjects.tree[category.Key.ToString()][metric.Key.ToString()] = metricObj;
              aSources[new StringPair(category.Key.ToString(), metric.Key.ToString())] = aFilepath;
            }
          }
        }
      }    
    }

    private static void _InstantiatePings(ref Metric.ObjectTree aAllObjects,
                                          Dictionary<object, string> aSources,
                                          YamlMappingNode aContent,
                                          string aFilepath,
                                          Dictionary<string, object> aConfig)
    {
      aAllObjects.tree["pings"] = new Dictionary<string, object>();

      foreach (var category in aContent.Children)
      {
        if (category.Key.ToString()[0].Equals('$'))
        {
          continue;
        }

        bool? allow_reserved = false;
        if (aConfig != null && aConfig.ContainsKey("allow_reserved"))
        {
          allow_reserved = aConfig["allow_reserved"] as bool?;
        }

        if (allow_reserved == false &&
          Ping.RESERVED_PING_NAMES.Contains(category.Key.ToString()))
        {
          Console.Error.WriteLine("{0}: For ping '{1}' " +
            "Ping uses a reserved name ({2}).", aFilepath, Ping.RESERVED_PING_NAMES);
          continue;
        }

        var pingVal = category.Value;
        if (pingVal.NodeType != YamlNodeType.Mapping)
        {
          throw new System.TypeLoadException(String.Format("Invalid content for ping {0}", category.Key));
        }
        else
        {
          var pingDict = new Dictionary<string, object>();
          var pingObj = Ping.MakePing(category.Key.ToString(), (YamlMappingNode)category.Value); //new Pings(category.Key, );

          if (aSources.ContainsKey(category.Key.ToString()))
          {
            Console.Error.WriteLine("{0}: Duplicate ping name '{1}'" +
              "already defined in '{2}'.", aFilepath, category.Key.ToString(), aSources[category.Key.ToString()]);
            continue;
          } else
          {
            aAllObjects.tree["pings"][category.Key.ToString()] = pingObj;
            aSources[category.Key.ToString()] = aFilepath;
          }
        }
      }
    }

    private static bool _loadFile(string aPath, ref YamlMappingNode aRootNode,
      ref string aFileType)
    {
      if (File.Exists(aPath) == false)
      {
        Console.Error.WriteLine("File doesn't exist: {0} ", aPath);
        return false;
      }

      string content = File.ReadAllText(aPath, Encoding.UTF8);

      if (content.Length == 0)
      {
        Console.Error.Write(aPath + " can't be empty.");
        return false;
      }

      var input = new StringReader(content);
      var yaml = new YamlStream();
      yaml.Load(input);

      var root =
              (YamlMappingNode)yaml.Documents[0].RootNode;
      var schemaKey = (YamlScalarNode)root.Children[new YamlScalarNode("$schema")];
      if (schemaKey == null)
      {
        Console.Error.Write("Invalid schema key: $schema");
        return false;
      }

      var key = schemaKey.Value;
      var fileType = FILE_TYPES[key];
      aRootNode = root;
      aFileType = fileType;
  
      return true;
    }
  }
}
