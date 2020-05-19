using Glean;
using System;
using System.Collections.Generic;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace Glean.Metrics
{
  public class PingType
  {
    private String _name;
    private bool _includeClientId;
    private bool _sendIfEmpty;
    private List<String> _reasonCodes = new List<String>();
    private UInt64 _handle;
    
    public PingType(String aName, bool aIncludeClientId, bool aSendIfEmpty, List<String> aReasonCodes)
    {
      _name = aName;
      _includeClientId = aIncludeClientId;
      _sendIfEmpty = aSendIfEmpty;
      _reasonCodes = aReasonCodes;
      _handle = Ffi.NewPingType(Ffi.EncodeString(aName), aIncludeClientId, aSendIfEmpty, Ffi.EncodeVectorString(aReasonCodes),
        aReasonCodes.Count);

      // TOD:
      Glean.RegisterPingType(this);
    }

    public UInt64 Handle()
    {
      return _handle;
    }
    // Destructor... destroy pings
  }
}
