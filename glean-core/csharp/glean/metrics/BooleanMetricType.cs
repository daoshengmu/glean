using System;

namespace Glean.Metrics
{
  internal sealed class BooleanMetricType
  {
    private bool disabled;
    private string[] sendInPings;
    private UInt64 handle;

    public BooleanMetricType(bool disabled,
        string category,
        Lifetime lifetime,
        string name,
        string[]  sendInPings) : this(disabled: disabled, sendInPings: sendInPings)
    {
      //  val ffiPingsList = StringArray(sendInPings.toTypedArray(), "utf-8")
      this.handle = LibGleanFFI.GleanNewBooleanMetric(
                category : category,
                name : name,
                send_in_pings: sendInPings,
                send_in_pings_len : sendInPings.Length,
                lifetime : lifetime,
                disabled : (disabled == true) ? (byte)1: (byte)0);
    }

    ~BooleanMetricType()
    {
      LibGleanFFI.GleanDestroyBooleanMetric(handle);
    }

    internal BooleanMetricType(bool disabled, string[] sendInPings)
    {
      this.disabled = disabled;
      this.sendInPings = sendInPings;
    }

    internal void SetSync(bool value)
    {
      if (disabled)
      {
        return;
       }

      LibGleanFFI.GleanBooleanSet(
          handle,
          value == true ? (byte)1: (byte)0
      );
    }
    internal bool TestHasValue(string pingName = null)
    {
      Dispatcher.AssertInTestingMode();

      string ping = pingName ?? sendInPings[0];
      return LibGleanFFI.GleanBooleanTestHasValue(this.handle, ping);
    }

    internal bool TestGetValue(string pingName = null) {
      Dispatcher.AssertInTestingMode();

      if (!TestHasValue(pingName)) {
        throw new NullReferenceException();
      }

      string ping = pingName ?? sendInPings[0];
      return LibGleanFFI.GleanBooleanTestGasValue(handle, ping);
    }
}
}
