using Glean;
using System;
using System.Collections.Generic;
using static Glean.Glean;

namespace Glean.Metrics
{
  public class PingType
  {
    private string _name;
    private bool _includeClientId;
    private bool _sendIfEmpty;
    private List<string> _reasonCodes = new List<string>();
    private UInt64 _handle;

    internal PingType(string aName, bool aIncludeClientId, bool aSendIfEmpty, List<string> aReasonCodes)
    {
      _name = aName;
      _includeClientId = aIncludeClientId;
      _sendIfEmpty = aSendIfEmpty;
      _reasonCodes = aReasonCodes;
      
      _handle = Ffi.NewPingType(aName, aIncludeClientId == true ? (byte)1 : (byte)0,
                                aSendIfEmpty == true ? (byte)1 : (byte)0, aReasonCodes,
        aReasonCodes.Count);

      // TODO:
      GleanInstance.RegisterPingType(this);
    }

    internal UInt64 Handle()
    {
      return _handle;
    }

    internal string Name()
    {
      return _name;
    }

    public void Submit(int? reason)
    {
      string reasonString = null;

      if (reason != null)
      {
        reasonString = _reasonCodes[(int)reason];
      }

      GleanInstance.SubmitPing(this, reasonString);
    }
  }
}
