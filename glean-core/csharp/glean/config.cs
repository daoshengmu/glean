
using Glean.Network;

namespace Glean
{
  public class Configuration {
    public const string DEFAULT_TELEMETRY_ENDPOINT = "https://incoming.telemetry.mozilla.org";
    // The default number of events to store before sending
    public const int DEFAULT_MAX_EVENTS = 500;

    public Configuration(string aServerEndpoint = DEFAULT_TELEMETRY_ENDPOINT,
                         string aUserAgent = null, string aChannel = null,
                         int aMaxEvents = DEFAULT_MAX_EVENTS, bool aLogPings = false,
                         string aPingTag = null, BaseUploader aBaseUpload = null,
                         bool aAllowMultiProcessing = true) {

    }
  }
}