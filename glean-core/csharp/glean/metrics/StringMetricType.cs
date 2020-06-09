using System;

namespace Glean.Metrics
{
  internal sealed class StringMetricType
  {
    private bool disabled;
    private string[] sendInPings;
    private UInt64 handle;

    public StringMetricType(bool disabled,
        string category,
        Lifetime lifetime,
        string name,
        string[] sendInPings)
    {
      this.disabled = disabled;
      this.sendInPings = sendInPings;

      handle = LibGleanFFI.GleanNewStringMetric(
           category,
           name,
           sendInPings,
           sendInPings.Length,
           lifetime,
           (disabled == true) ? (byte)1 : (byte)0
       );
    }

    ~StringMetricType()
    {
      LibGleanFFI.GleanDestroyStringMetric(handle);
    }

    public void SetSync(string value)
    {
      LibGleanFFI.glean_string_set(handle, value);
    }

    public bool TestHasValue(string pingName = null)
    {
      Dispatcher.AssertInTestingMode();

      string ping = pingName ?? sendInPings[0];
      return LibGleanFFI.GleanStringTestHasValue(this.handle, ping);
    }

    public string TestGetValue(string pingName = null)
    {
      Dispatcher.AssertInTestingMode();

      if (!TestHasValue(pingName))
      {
        throw new NullReferenceException();
      }

      string ping = pingName ?? sendInPings[0];
      return LibGleanFFI.GleanStringTestGasValue(this.handle, ping);
    }
  }
}
