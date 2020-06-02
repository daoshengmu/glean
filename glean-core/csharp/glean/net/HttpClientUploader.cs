using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Glean.Net
{
  using HeadersList = List<KeyValuePair<string, string>>;

  internal class HttpClientUploader : PingUploader
  {
    private readonly int DEFAULT_CONNECTION_TIMEOUT = 10000;

    internal override async Task<UploadResult> Upload(string aUrl, byte[] aData,
      HeadersList aHeaders)
    {
      var client = new HttpClient();
      client.Timeout = TimeSpan.FromMilliseconds(DEFAULT_CONNECTION_TIMEOUT);

      var json = JsonConvert.SerializeObject(aHeaders);
      var data = new StringContent(json, Encoding.UTF8, "application/json");
      var response = await client.PostAsync(aUrl, data);

      // TOOD: Need to remove cookie?

      return new HttpResponse(response.StatusCode);
    }
    

  }
}
