using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using static Glean.Glean;

namespace Glean.Net
{
  using HeadersList = List<KeyValuePair<String, String>>;
  internal delegate void PingUploadTask();

  internal class FfiPingUploadTask
  {
    enum UploadTaskTag : byte
    {
      Upload = 0,
      Wait = 1,
      Done = 2
    }

    internal class PingRequest
    {
      private string documentId;
      internal string path;
      internal byte[] body;
      internal HeadersList headers;

      internal PingRequest(string aDocumentId, string aPath,
        byte[] aBody, string aHeaders)
      {
        documentId = aDocumentId;
        path = aPath;
        body = (byte[])aBody.Clone();
        headers = HeadersFromJSONString(aHeaders);
      }

      private HeadersList HeadersFromJSONString(string aHeaders)
      {
        try
        {
          var jsonHeaders = JsonConvert.DeserializeObject<Dictionary<string, string>>(aHeaders);
          foreach (var key in jsonHeaders.Keys)
          {
            headers.Add(new KeyValuePair<string, string>(key, jsonHeaders[key].ToString()));
          }
        }
        catch (JsonException)
        {
          Console.Error.WriteLine("Error while parsing headers for ping $documentId");
        }

        if (GleanInstance.configuration.pingTag != null)
        {
          headers.Add(new KeyValuePair<string, string>("X-Debug-ID", GleanInstance.configuration.pingTag));
        }

        return headers;
      }
    }

    private class UploadBody
    {
      byte tag = (byte)UploadTaskTag.Done;
      string documentId = null;
      string path = null;
      byte[] body;
      string headers = null;

      internal PingRequest ToPingRequest()
      {
        return new PingRequest(
            this.documentId != null ? this.documentId : "",
            this.path != null ? this.path : "",
            this.body,
            this.headers != null ? this.headers : ""
        );
      }
    }


    private byte _tag = (byte)UploadTaskTag.Done;
    private UploadBody _upload = new UploadBody();

    internal PingUploadTask ToPingUploadTask()
    {
      switch (this._tag)
      {
        case (byte)UploadTaskTag.Wait:
          return new PingUploadTaskWait();
        case (byte)UploadTaskTag.Upload:
          return new PingUploadTaskUpload(this._upload.ToPingRequest());
        case (byte)UploadTaskTag.Done:
          return new PingUploadTaskDone();

      }
      return null;
    }

    internal class PingUploadTask
    {

    }

    internal class PingUploadTaskUpload : PingUploadTask
    {
      internal PingRequest request;

      internal PingUploadTaskUpload(PingRequest aRequest)
      {
        request = aRequest;
      }
    }

    internal class PingUploadTaskWait : PingUploadTask
    {
      internal PingUploadTaskWait()
      {

      }
    }

    internal class PingUploadTaskDone : PingUploadTask
    {
      internal PingUploadTaskDone()
      {

      }
    }
  }
}
