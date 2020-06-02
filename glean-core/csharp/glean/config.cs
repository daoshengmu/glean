
using Glean.Net;
using System;
using System.Reflection;

namespace Glean
{
  public class Configuration {
    public const string DEFAULT_TELEMETRY_ENDPOINT = "https://incoming.telemetry.mozilla.org";
    // The default number of events to store before sending
    public const int DEFAULT_MAX_EVENTS = 500;

    public string serverEndpoint = DEFAULT_TELEMETRY_ENDPOINT;
    public string agent = null;
    public string channel = null;
    public int maxEvents = DEFAULT_MAX_EVENTS;
    public bool logPings = false;
    public string pingTag = null;
    internal BaseUploader pingUploader = null;
    public bool allowMultiprocessing = true;


    public Configuration(string aServerEndpoint = DEFAULT_TELEMETRY_ENDPOINT,
                         string aUserAgent = null, string aChannel = null,
                         int aMaxEvents = DEFAULT_MAX_EVENTS, bool aLogPings = false,
                         string aPingTag = null, BaseUploader aPingUpload = null,
                         bool aAllowMultiProcessing = true) {

      serverEndpoint = aServerEndpoint;

      if (aUserAgent == null)
      {
        agent = GetDefaultUserAgent();
      } else
      {
        agent = aUserAgent;
      }

      channel = aChannel;
      maxEvents = aMaxEvents;
      logPings = aLogPings;
      pingTag = aPingTag;

      if (aPingUpload == null)
      {
        pingUploader = new BaseUploader(new HttpClientUploader());
      } else
      {
        pingUploader = aPingUpload;
      }

      allowMultiprocessing = aAllowMultiProcessing;
    }

    private string GetDefaultUserAgent()
    {
      // TODO: Making plaform match to Python's?
      return string.Format("Glean/{0} (C# on {1})",
        Assembly.GetExecutingAssembly().GetName().Version, Environment.OSVersion.Platform);
    }
  }
}