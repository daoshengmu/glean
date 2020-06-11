﻿// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using Mozilla.Glean.FFI;
using System;

namespace Mozilla.Glean.Private
{
    public sealed class UuidMetricType
    {
        private bool disabled;
        private string[] sendInPings;
        private LibGleanFFI.UuidMetricTypeHandle handle;

        public UuidMetricType(
            bool disabled,
            string category,
            Lifetime lifetime,
            string name,
            string[] sendInPings
            ) : this(new LibGleanFFI.UuidMetricTypeHandle(), disabled, sendInPings)
        {
            handle = LibGleanFFI.glean_new_uuid_metric(
                       category: category,
                       name: name,
                       send_in_pings: sendInPings,
                       send_in_pings_len: sendInPings.Length,
                       lifetime: (int)lifetime,
                       disabled: disabled);
        }

        internal UuidMetricType(
           LibGleanFFI.UuidMetricTypeHandle handle,
           bool disabled,
           string[] sendInPings
           )
        {
            this.disabled = disabled;
            this.sendInPings = sendInPings;
            this.handle = handle;
        }

        /// <summary>
        /// Generate a new UUID value and set it in the metric store.
        /// </summary>
        public Guid? GenerateAndSet()
        {
            // Even if `set` is already checking if we're allowed to record,
            // we need to check here as well otherwise we'd return a `UUID`
            // that won't be stored anywhere.
            if (disabled) {
                return null;
            }

            var uuid = System.Guid.NewGuid();
            Set(uuid);
            return uuid;
        }

        /// <summary>
        /// Explicitly set an existing UUID value.
        /// </summary>
        /// <param name="value"> A valid [UUID] to set the metric to.
        public void Set(Guid value)
        {
            if (disabled)
            {
                return;
            }

            Dispatchers.LaunchAPI(() =>
            {
                SetSync(value);
            });
        }

        /// <summary>
        /// Internal only, synchronous API for setting a UUID value.
        /// </summary>
        /// <param name="value">This is a user defined boolean value.</param>
        internal void SetSync(Guid value)
        {
            LibGleanFFI.glean_uuid_set(this.handle, value.ToString());
        }

        /// <summary>
        /// Tests whether a value is stored for the metric for testing purposes only. This function will
        /// attempt to await the last task (if any) writing to the the metric's storage engine before
        /// returning a value.
        /// </summary>
        /// <param name="pingName">represents the name of the ping to retrieve the metric for Defaults
        /// to the first value in `sendInPings`</param>
        /// <returns>true if metric value exists, otherwise false</returns>
        public bool TestHasValue(string pingName = null)
        {
            Dispatchers.AssertInTestingMode();

            var ping = pingName ?? sendInPings[0];
            return LibGleanFFI.glean_uuid_test_has_value(this.handle, ping) != 0;
        }

        /// <summary>
        /// Returns the stored value for testing purposes only. This function will attempt to await the
        /// last task (if any) writing to the the metric's storage engine before returning a value.
        /// @throws [NullPointerException] if no value is stored
        /// </summary>
        /// <param name="pingName">represents the name of the ping to retrieve the metric for.
        /// Defaults to the first value in `sendInPings`</param>
        /// <returns>value of the stored metric</returns>
        /// <exception cref="System.NullReferenceException">Thrown when the metric contains no value</exception>
        public Guid TestGetValue(string pingName = null)
        {
            Dispatchers.AssertInTestingMode();

            if (!TestHasValue(pingName))
            {
                throw new NullReferenceException();
            }

            var ping = pingName ?? sendInPings[0];
            return Guid.Parse(LibGleanFFI.glean_uuid_test_get_value(this.handle, ping).AsString());
        }
    }
}
