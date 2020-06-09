using System;
using System.Collections.Generic;
using System.Text;

namespace Glean.Metrics.ManualMetrics
{
  internal sealed class GleanInternalMetricsOuter
  {
    // Initialize the singleton using the `Lazy` facilities.
    private static readonly Lazy<GleanInternalMetricsOuter>
      lazy = new Lazy<GleanInternalMetricsOuter>(() => new GleanInternalMetricsOuter());

    public static GleanInternalMetricsOuter GleanInternalMetrics => lazy.Value;

    // Private constructor to disallow instantiation from external callers.
    private GleanInternalMetricsOuter() { }

    private readonly Lazy<StringMetricType> architectureLazy = new Lazy<StringMetricType>(() => new StringMetricType(
            category: "",
            disabled: false,
            lifetime: Lifetime.Application,
            name: "architecture",
            sendInPings: new string[] { "glean_client_info" }
        ));

    public StringMetricType architecture => architectureLazy.Value;
    public BooleanMetricType status => statusLazy.Value;

    private readonly Lazy<BooleanMetricType> statusLazy = new Lazy<BooleanMetricType>(() => new BooleanMetricType(
         category: "",
         disabled: false,
         lifetime: Lifetime.Application,
         name: "glean_new_boolean_metric",
         sendInPings: new string[] { "glean_client_info" }
     ));
  }
}
