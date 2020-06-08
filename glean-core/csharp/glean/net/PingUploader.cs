using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using static Glean.LibGleanFFI;

namespace Glean.Net
{
  internal class UploadResult
  {
    internal virtual int ToFfi() {
      return Constants.UPLOAD_RESULT_UNRECOVERABLE;
    }
  }

  internal class HttpResponse : UploadResult
  {
    private HttpStatusCode statusCode;

    internal HttpResponse(HttpStatusCode aStatusCode)
    {
      statusCode = aStatusCode;
    }

    internal override int ToFfi() {
      return Constants.UPLOAD_RESULT_HTTP_STATUS | (int)statusCode;
    }
  }

  public abstract class PingUploader
  {
    internal abstract Task<UploadResult> Upload(string aUrl, byte[] aData,
      List<KeyValuePair<string, string>> aHeaders);
  }
}
