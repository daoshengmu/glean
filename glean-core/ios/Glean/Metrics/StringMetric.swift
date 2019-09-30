/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

import Foundation

/// This implements the developer facing API for recording string metrics.
///
/// Instances of this class type are automatically generated by the parsers at build time,
/// allowing developers to record values that were previously registered in the metrics.yaml file.
///
/// The string API only exposes the `StringMetricType.set(_:)` method, which takes care of validating the input
/// data and making sure that limits are enforced.
public class StringMetricType {
    let handle: UInt64
    let disabled: Bool
    let sendInPings: [String]

    /// The public constructor used by automatically generated metrics.
    public init(category: String, name: String, sendInPings: [String], lifetime: Lifetime, disabled: Bool) {
        self.disabled = disabled
        self.sendInPings = sendInPings
        self.handle = withArrayOfCStrings(sendInPings) { pingArray in
            glean_new_string_metric(
                category,
                name,
                pingArray,
                Int32(sendInPings.count),
                lifetime.rawValue,
                disabled ? 1 : 0
            )
        }
    }

    /// An internal constructor to be used by the `LabeledMetricType` directly.
    init(withHandle handle: UInt64, disabled: Bool, sendInPings: [String]) {
        self.handle = handle
        self.disabled = disabled
        self.sendInPings = sendInPings
    }

    /// Destroy this metric.
    deinit {
        if self.handle != 0 {
            glean_destroy_string_metric(self.handle)
        }
    }

    /// Set a string value.
    ///
    /// - parameters:
    ///     * value: This is a user defined string value. If the length of the string exceeds
    ///              the maximum length, it will be truncated.
    public func set(_ value: String) {
        guard !self.disabled else { return }

        Dispatchers.shared.launchAPI {
            self.setSync(value)
        }
    }

    /// Internal only, synchronous API for setting a string value.
    func setSync(_ value: String) {
        glean_string_set(Glean.shared.handle, self.handle, value)
    }

    /// Tests whether a value is stored for the metric for testing purposes only. This function will
    /// attempt to await the last task (if any) writing to the the metric's storage engine before
    /// returning a value.
    ///
    /// - parameters:
    ///     * pingName: represents the name of the ping to retrieve the metric for.
    ///                 Defaults to the first value in `sendInPings`.
    /// - returns: true if metric value exists, otherwise false
    func testHasValue(_ pingName: String? = nil) -> Bool {
        Dispatchers.shared.assertInTestingMode()

        let pingName = pingName ?? self.sendInPings[0]
        return glean_string_test_has_value(Glean.shared.handle, self.handle, pingName) != 0
    }

    /// Returns the stored value for testing purposes only. This function will attempt to await the
    /// last task (if any) writing to the the metric's storage engine before returning a value.
    ///
    /// Throws a `String` exception if no value is stored
    ///
    /// - parameters:
    ///     * pingName: represents the name of the ping to retrieve the metric for.
    ///                 Defaults to the first value in `sendInPings`.
    ///
    /// - returns:  value of the stored metric
    func testGetValue(_ pingName: String? = nil) throws -> String {
        Dispatchers.shared.assertInTestingMode()

        let pingName = pingName ?? self.sendInPings[0]

        if !testHasValue(pingName) {
            throw "Missing value"
        }

        return String(freeingRustString: glean_string_test_get_value(Glean.shared.handle, self.handle, pingName))
    }
}
