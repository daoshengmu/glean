using System;

namespace Glean.Metrics
{
  internal enum Lifetime
  {
    /// <summary>
    /// The metric is reset with each sent ping
    /// </summary>
    Ping,
    /// <summary>
    /// The metric is reset on application restart
    /// </summary>
    Application,
    /// <summary>
    /// The metric is reset with each user profile
    /// </summary>
    User
  }
}
