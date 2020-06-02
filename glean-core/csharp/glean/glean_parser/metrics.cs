using System;
using System.Collections.Generic;
using System.Diagnostics;
using YamlDotNet.RepresentationModel;

namespace Glean.glean_parser
{
  public class Metric
  {
    public const string GleanInternalMetricCat = "glean.internal.metrics";
   // protected string typename = "ERROR";
 
    public static List<string> DefaultStoreNames = new List<string> { "metrics" };

    public string type;
    public string category;
    public string name;
    public List<string> bugs = new List<string>();
    public string description;
    public List<string> notification_emails = new List<string>();
    public string expires;
    public List<string> data_reviews = null;
    public int version = 0;
    public bool disabled = false;
    public string lifetime = "ping";
    public List<string> send_in_pings = null;
    public string unit = "";
    public string gecko_datapoint = "";
    public List<string> noLint = null;
    private Dictionary<string, object> _config  = null;
    private bool _validated = false;
    private object _value;

    public static Metric MakeMetric(string aCategory, string aName,
      YamlMappingNode aMetricInfo, Dictionary<string, object> aConfig, bool aValidated = false)
    {
      //"""
      //Given a metric_info dictionary from metrics.yaml, return a metric
      //instance.

      //:param: category The category the metric lives in
      //:param: name The name of the metric
      //:param: metric_info A dictionary of the remaining metric parameters
      //:param: config A dictionary containing commandline configuration
      //    parameters
      //:param: validated True if the metric has already gone through
      //    jsonschema validation
      //:return: A new Metric instance.
      // """
      Metric metric = null;
      if (aMetricInfo.Children.ContainsKey("type"))
      {
        var metricType = aMetricInfo["type"]; //.Children[new YamlScalarNode("type")];
        if (metricType.ToString().Length != 0)
        {
          metric = InstanceMetricByType(metricType.ToString(), aCategory,
            aName, aValidated, aConfig, aMetricInfo);
        }
      }
      
      return metric;
    }

    public Metric(string aType,
        string aCategory,
        string aName,
        List<string> aBugs,
        string aDescription,
        List<string> aNotificationEmails,
        string aExpires,
        List<string> aDataReviews = null,
        int aVersion = 0,
        bool aDisabled = false,
        string aLifetime = "ping",
        List<string> aSendInPings = null,
        string aUnit = "",
        string aGeckoDatapoint = "",
        List<string> aNoLint = null,
        Dictionary<string, object> aConfig = null,
        bool aValidated = false)
    {
      type = aType;
      category = aCategory;
      name = aName;
      bugs = aBugs;
      description = aDescription;
      notification_emails = aNotificationEmails;
      expires = aExpires;
      data_reviews = aDataReviews;
      version = aVersion;
      disabled = aDisabled;
      lifetime = aLifetime;
      send_in_pings = aSendInPings;
      unit = aUnit;
      gecko_datapoint = aGeckoDatapoint;
      noLint = aNoLint;
      _config = aConfig;
      _validated = aValidated;
    }

    public void Set(object aValue)
    {
      _value = aValue;
    }

    public virtual void Add(int aCount)
    {

    }

    private static Metric InstanceMetricByType(string aType, string aCategory,
      string aName, bool aValidated, Dictionary<string, object> aConfig, YamlMappingNode aMetricInfo)
    {
      switch(aType)
      {
        case "boolean":
          return new BooleanMetric(aType: aType, aCategory:aCategory, aName: aName,
            aValidated: aValidated, aConfig: aConfig);
        case "string":
          return new StringMetric(aType: aType, aCategory: aCategory, aName: aName,
            aValidated: aValidated, aConfig: aConfig);
        case "counter":
          return new CounterMetric(aType: aType, aCategory: aCategory, aName: aName,
            aValidated: aValidated, aConfig: aConfig);
        case "quantity":
          return new QuantityMetric(aType: aType, aCategory: aCategory, aName: aName,
            aValidated: aValidated, aConfig: aConfig);
        case "timespan":
          return new TimeSpanMetric(aType: aType, aCategory: aCategory, aName: aName,
            aValidated: aValidated, aConfig: aConfig, aMetricInfo: aMetricInfo);
        case "timing_distribution":
          return new TimingDistributionMetric(aType: aType, aCategory: aCategory, aName: aName,
            aValidated: aValidated, aConfig: aConfig, aMetricInfo: aMetricInfo);
        case "custom_distribution":
          return new CustomDistributionMetric(aType: aType, aCategory: aCategory, aName: aName,
            aValidated: aValidated, aConfig: aConfig);
        case "datetime":
          return new DatetimeMetric(aType: aType, aCategory: aCategory, aName: aName,
            aValidated: aValidated, aConfig: aConfig);
        case "uuid":
          return new UuidMetric(aType: aType, aCategory: aCategory, aName: aName,
            aValidated: aValidated, aConfig: aConfig);
        case "labeled_counter":
          return new  LabeledCounterMetric(aType: aType, aCategory: aCategory, aName: aName,
            aValidated: aValidated, aConfig: aConfig);
        default:
          Console.Error.WriteLine("Undefined metric type. {0}", aType);
          Debug.Assert(false);
          break;
      }
      return null;
    }

    class BooleanMetric : Metric
    {
      public BooleanMetric(string aType,
        string aCategory,
        string aName,
        List<string> aBugs = null,
        string aDescription = "",
        List<string> aNotificationEmails = null,
        string aExpires = "",
        List<string> aDataReviews = null,
        int aVersion = 0,
        bool aDisabled = false,
        string aLifetime = "ping",
        List<string> aSendInPings = null,
        string aUnit = "",
        string aGeckoDatapoint = "",
        List<string> aNoLint = null,
        Dictionary<string, object> aConfig = null,
        bool aValidated = false) :
        base(aType, aCategory, aName, aBugs, aDescription, aNotificationEmails,
          aExpires, aDataReviews, aVersion, aDisabled, aLifetime, aSendInPings,
          aUnit, aGeckoDatapoint, aNoLint, aConfig, aValidated)
      {
        
      }
    }

    class StringMetric : Metric
    {
      public StringMetric(string aType,
        string aCategory,
        string aName,
        List<string> aBugs = null,
        string aDescription = "",
        List<string> aNotificationEmails = null,
        string aExpires = "",
        List<string> aDataReviews = null,
        int aVersion = 0,
        bool aDisabled = false,
        string aLifetime = "ping",
        List<string> aSendInPings = null,
        string aUnit = "",
        string aGeckoDatapoint = "",
        List<string> aNoLint = null,
        Dictionary<string, object> aConfig = null,
        bool aValidated = false) :
        base(aType, aCategory, aName, aBugs, aDescription, aNotificationEmails,
          aExpires, aDataReviews, aVersion, aDisabled, aLifetime, aSendInPings,
          aUnit, aGeckoDatapoint, aNoLint, aConfig, aValidated)
      {
 
      }
    }

    class CounterMetric : Metric
    {
      public CounterMetric(string aType,
        string aCategory,
        string aName,
        List<string> aBugs = null,
        string aDescription = "",
        List<string> aNotificationEmails = null,
        string aExpires = "",
        List<string> aDataReviews = null,
        int aVersion = 0,
        bool aDisabled = false,
        string aLifetime = "ping",
        List<string> aSendInPings = null,
        string aUnit = "",
        string aGeckoDatapoint = "",
        List<string> aNoLint = null,
        Dictionary<string, object> aConfig = null,
        bool aValidated = false) :
        base(aType, aCategory, aName, aBugs, aDescription, aNotificationEmails,
          aExpires, aDataReviews, aVersion, aDisabled, aLifetime, aSendInPings,
          aUnit, aGeckoDatapoint, aNoLint, aConfig, aValidated)
      {

      }
    }

    class QuantityMetric : Metric
    {
      public QuantityMetric(string aType,
        string aCategory,
        string aName,
        List<string> aBugs = null,
        string aDescription = "",
        List<string> aNotificationEmails = null,
        string aExpires = "",
        List<string> aDataReviews = null,
        int aVersion = 0,
        bool aDisabled = false,
        string aLifetime = "ping",
        List<string> aSendInPings = null,
        string aUnit = "",
        string aGeckoDatapoint = "",
        List<string> aNoLint = null,
        Dictionary<string, object> aConfig = null,
        bool aValidated = false) :
        base(aType, aCategory, aName, aBugs, aDescription, aNotificationEmails,
          aExpires, aDataReviews, aVersion, aDisabled, aLifetime, aSendInPings,
          aUnit, aGeckoDatapoint, aNoLint, aConfig, aValidated)
      {

      }
    }

    enum TimeUnit
    {
      nanosecond = 0,
      microsecond = 1,
      millisecond = 2,
      second = 3,
      minute = 4,
      hour = 5,
      day = 6
    }

    TimeUnit GetTimeUnitByString(string aType)
    {
      TimeUnit result = TimeUnit.nanosecond;
      switch(aType)
      {
        case "nanosecond":
          result = TimeUnit.nanosecond;
          break;
        case "microsecond":
          result = TimeUnit.microsecond;
          break;
        case "millisecond":
          result = TimeUnit.millisecond;
          break;
        case "second":
          result = TimeUnit.second;
          break;
        case "minute":
          result = TimeUnit.minute;
          break;
        case "hour":
          result = TimeUnit.hour;
          break;
        case "day":
          result = TimeUnit.day;
          break;
        default:
          Console.Error.WriteLine("Undefined time unit. {0}", aType);
          Debug.Assert(false);
          break;
      }

      return result;
    }

    class TimeBaseMetric : Metric
    {
      protected TimeUnit time_unit = TimeUnit.millisecond;

      public TimeBaseMetric(string aType,
       string aCategory,
       string aName,
       List<string> aBugs = null,
       string aDescription = "",
       List<string> aNotificationEmails = null,
       string aExpires = "",
       List<string> aDataReviews = null,
       int aVersion = 0,
       bool aDisabled = false,
       string aLifetime = "ping",
       List<string> aSendInPings = null,
       string aUnit = "",
       string aGeckoDatapoint = "",
       List<string> aNoLint = null,
       Dictionary<string, object> aConfig = null,
       bool aValidated = false,
       YamlMappingNode aMetricInfo = null) :
       base(aType, aCategory, aName, aBugs, aDescription, aNotificationEmails,
         aExpires, aDataReviews, aVersion, aDisabled, aLifetime, aSendInPings,
         aUnit, aGeckoDatapoint, aNoLint, aConfig, aValidated)
      {
        if (aMetricInfo != null)
        {
          if (aMetricInfo.Children.ContainsKey("time_unit"))
          {
            var item = aMetricInfo.Children[new YamlScalarNode("time_unit")];
            time_unit = GetTimeUnitByString(item.ToString());
          }
        }
      }
    }

    class TimeSpanMetric : TimeBaseMetric
    {
      public TimeSpanMetric(string aType,
        string aCategory,
        string aName,
        List<string> aBugs = null,
        string aDescription = "",
        List<string> aNotificationEmails = null,
        string aExpires = "",
        List<string> aDataReviews = null,
        int aVersion = 0,
        bool aDisabled = false,
        string aLifetime = "ping",
        List<string> aSendInPings = null,
        string aUnit = "",
        string aGeckoDatapoint = "",
        List<string> aNoLint = null,
        Dictionary<string, object> aConfig = null,
        bool aValidated = false,
        YamlMappingNode aMetricInfo = null) :
        base(aType, aCategory, aName, aBugs, aDescription, aNotificationEmails,
          aExpires, aDataReviews, aVersion, aDisabled, aLifetime, aSendInPings,
          aUnit, aGeckoDatapoint, aNoLint, aConfig, aValidated, aMetricInfo)
      { 
      
      }
    }

    class TimingDistributionMetric: TimeBaseMetric
    {
      public TimingDistributionMetric(string aType,
        string aCategory,
        string aName,
        List<string> aBugs = null,
        string aDescription = "",
        List<string> aNotificationEmails = null,
        string aExpires = "",
        List<string> aDataReviews = null,
        int aVersion = 0,
        bool aDisabled = false,
        string aLifetime = "ping",
        List<string> aSendInPings = null,
        string aUnit = "",
        string aGeckoDatapoint = "",
        List<string> aNoLint = null,
        Dictionary<string, object> aConfig = null,
        bool aValidated = false,
        YamlMappingNode aMetricInfo = null) :
        base(aType, aCategory, aName, aBugs, aDescription, aNotificationEmails,
          aExpires, aDataReviews, aVersion, aDisabled, aLifetime, aSendInPings,
          aUnit, aGeckoDatapoint, aNoLint, aConfig, aValidated, aMetricInfo)
      {
        time_unit = TimeUnit.nanosecond;
      }
    }

    enum MemoryUnit
    {
      byte_ = 0,
      kilobyte = 1,
      megabyte = 2,
      gigabyte = 3,
    }

    class MemoryDistributionMetric: Metric
    {
      public MemoryDistributionMetric(string aType,
       string aCategory,
       string aName,
       List<string> aBugs = null,
       string aDescription = "",
       List<string> aNotificationEmails = null,
       string aExpires = "",
       List<string> aDataReviews = null,
       int aVersion = 0,
       bool aDisabled = false,
       string aLifetime = "ping",
       List<string> aSendInPings = null,
       string aUnit = "",
       string aGeckoDatapoint = "",
       List<string> aNoLint = null,
       Dictionary<string, object> aConfig = null,
       bool aValidated = false) :
       base(aType, aCategory, aName, aBugs, aDescription, aNotificationEmails,
         aExpires, aDataReviews, aVersion, aDisabled, aLifetime, aSendInPings,
         aUnit, aGeckoDatapoint, aNoLint, aConfig, aValidated)
      {
      }
    }

    enum HistogramType
    {
      linear = 0,
      exponential = 1,
    }

    class CustomDistributionMetric : Metric
    {
      public int range_min = 0; //
      public int range_max = 0;  //
      public int bucket_count = 0; //
      public HistogramType histogram_type = HistogramType.exponential;

      public CustomDistributionMetric(string aType,
       string aCategory,
       string aName,
       List<string> aBugs = null,
       string aDescription = "",
       List<string> aNotificationEmails = null,
       string aExpires = "",
       List<string> aDataReviews = null,
       int aVersion = 0,
       bool aDisabled = false,
       string aLifetime = "ping",
       List<string> aSendInPings = null,
       string aUnit = "",
       string aGeckoDatapoint = "",
       List<string> aNoLint = null,
       Dictionary<string, object> aConfig = null,
       bool aValidated = false) :
       base(aType, aCategory, aName, aBugs, aDescription, aNotificationEmails,
         aExpires, aDataReviews, aVersion, aDisabled, aLifetime, aSendInPings,
         aUnit, aGeckoDatapoint, aNoLint, aConfig, aValidated)
      {
      }
    }

    class DatetimeMetric : TimeBaseMetric
    {
      public DatetimeMetric(string aType,
        string aCategory,
        string aName,
        List<string> aBugs = null,
        string aDescription = "",
        List<string> aNotificationEmails = null,
        string aExpires = "",
        List<string> aDataReviews = null,
        int aVersion = 0,
        bool aDisabled = false,
        string aLifetime = "ping",
        List<string> aSendInPings = null,
        string aUnit = "",
        string aGeckoDatapoint = "",
        List<string> aNoLint = null,
        Dictionary<string, object> aConfig = null,
        bool aValidated = false) :
        base(aType, aCategory, aName, aBugs, aDescription, aNotificationEmails,
          aExpires, aDataReviews, aVersion, aDisabled, aLifetime, aSendInPings,
          aUnit, aGeckoDatapoint, aNoLint, aConfig, aValidated)
      {
      }
    }

    // TODO: Event...
    class UuidMetric : Metric
    {
      public UuidMetric(string aType,
        string aCategory,
        string aName,
        List<string> aBugs = null,
        string aDescription = "",
        List<string> aNotificationEmails = null,
        string aExpires = "",
        List<string> aDataReviews = null,
        int aVersion = 0,
        bool aDisabled = false,
        string aLifetime = "ping",
        List<string> aSendInPings = null,
        string aUnit = "",
        string aGeckoDatapoint = "",
        List<string> aNoLint = null,
        Dictionary<string, object> aConfig = null,
        bool aValidated = false) :
        base(aType, aCategory, aName, aBugs, aDescription, aNotificationEmails,
          aExpires, aDataReviews, aVersion, aDisabled, aLifetime, aSendInPings,
          aUnit, aGeckoDatapoint, aNoLint, aConfig, aValidated)
      {
      }
    }

    class LabeledMetric : Metric
    {
      public static bool labeled = true;
      public List<string> ordered_labels = null;
      public List<string> labels = null;

      public LabeledMetric(string aType,
        string aCategory,
        string aName,
        List<string> aBugs = null,
        string aDescription = "",
        List<string> aNotificationEmails = null,
        string aExpires = "",
        List<string> aDataReviews = null,
        int aVersion = 0,
        bool aDisabled = false,
        string aLifetime = "ping",
        List<string> aSendInPings = null,
        string aUnit = "",
        string aGeckoDatapoint = "",
        List<string> aNoLint = null,
        Dictionary<string, object> aConfig = null,
        bool aValidated = false,
        YamlMappingNode aMetricInfo = null) :
        base(aType, aCategory, aName, aBugs, aDescription, aNotificationEmails,
          aExpires, aDataReviews, aVersion, aDisabled, aLifetime, aSendInPings,
          aUnit, aGeckoDatapoint, aNoLint, aConfig, aValidated)
      {
        if (aMetricInfo != null)
        {
          if (aMetricInfo.Children.ContainsKey("labels"))
          {
            ordered_labels = new List<string>();
            labels = new List<string>();
            var items = (YamlSequenceNode)aMetricInfo.Children[new YamlScalarNode("labels")];
            foreach (var item in items)
            {
              ordered_labels.Add(item.ToString());
              labels.Add(item.ToString());
            }
          }
        }
      }
    }

    class LabeledCounterMetric : LabeledMetric
    {
      private int _counter = 0;

      public LabeledCounterMetric(string aType,
        string aCategory,
        string aName,
        List<string> aBugs = null,
        string aDescription = "",
        List<string> aNotificationEmails = null,
        string aExpires = "",
        List<string> aDataReviews = null,
        int aVersion = 0,
        bool aDisabled = false,
        string aLifetime = "ping",
        List<string> aSendInPings = null,
        string aUnit = "",
        string aGeckoDatapoint = "",
        List<string> aNoLint = null,
        Dictionary<string, object> aConfig = null,
        bool aValidated = false) :
        base(aType, aCategory, aName, aBugs, aDescription, aNotificationEmails,
          aExpires, aDataReviews, aVersion, aDisabled, aLifetime, aSendInPings,
          aUnit, aGeckoDatapoint, aNoLint, aConfig, aValidated)
      {
      }

      public override void Add(int aCounter)
      {
        _counter += aCounter;
      }
    }

    public class ObjectTree
    {
      // object is Metrics or Pings.ping
      public Dictionary<string, Dictionary<string, object>> tree =
        new Dictionary<string, Dictionary<string, object>>();
    }
  }
}
