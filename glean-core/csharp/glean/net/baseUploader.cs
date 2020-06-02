
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Glean.Net
{
  using HeadersList = List<KeyValuePair<string, string>>;

  public class BaseUploader
  {
    private PingUploader _uploader;

    public BaseUploader(PingUploader aUploader)
    {
      _uploader = aUploader;
    }

    private void LogPing(string aPath, string data)
    {
      // Parse and reserialize the JSON so it has indentation and is human-readable.
    }

    internal UploadResult DoUpload(string aPath, byte[] aData,
      HeadersList aHeaders, Configuration config)
    {
      if (config.logPings)
      {
        // TODO: Support Log the contents of a ping to the console.
        LogPing(aPath, aData.ToString());
      }
      return _uploader.Upload(config.serverEndpoint + aPath, aData, aHeaders).Result;
    }
  }
}
