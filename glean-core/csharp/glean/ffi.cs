
using Glean.Net;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Glean {
  internal sealed class Ffi {

    [DllImport(_libFileName)]
    private static extern void glean_enable_logging();

    [DllImport(_libFileName, CallingConvention = CallingConvention.Cdecl)]
    private static extern byte glean_initialize(FfiConfiguration cfg);

    [StructLayout(LayoutKind.Sequential)]
    internal sealed class FfiConfiguration
    {
      internal string data_dir;
      internal string package_name;
      internal bool upload_enabled;
      internal Int32 max_events;
      internal bool delay_ping_lifetime_io;
    }

    // TODO: Check if aPingName and aReasonCodes work
    // (https://github.com/mozilla/glean/blob/61c542ed822e39b589fcc5b43d824d8bbf5ea693/glean-core/ffi/src/ping_type.rs#L19)
    [DllImport(_libFileName)]
    private static extern UInt64 glean_new_ping_type(string aPingName, byte aIncludeClientId, byte aSendIfEmpty,
                                                     string[] aReasonCodes, int aReasonCodesLen);

    [DllImport(_libFileName)]
    private static extern void glean_register_ping_type(UInt64 aPingTypeHandle);

    [DllImport(_libFileName)]
    private static extern void glean_set_upload_enabled(byte aFlag);

    [DllImport(_libFileName)]
    private static extern byte glean_is_upload_enabled();

    [DllImport(_libFileName)]
    private static extern byte glean_on_ready_to_submit_pings();

    [DllImport(_libFileName)]
    private static extern byte glean_submit_ping_by_name(string aPingName, string aReason);

    [DllImport(_libFileName)]
    private static extern void glean_get_upload_task(ref FfiPingUploadTask aUploadTask);

    [DllImport(_libFileName)]
    private static extern byte glean_is_first_run();

    [DllImport(_libFileName)]
    private static extern void glean_clear_application_lifetime_metrics();

    [DllImport(_libFileName)]
    private static extern void glean_get_upload_task(ref FfiPingUploadTask result, byte log_ping);


    internal sealed class Constants
    {
      // A recoverable error.
      static internal int UPLOAD_RESULT_RECOVERABLE = 0x1;

      // An unrecoverable error.
      static internal int UPLOAD_RESULT_UNRECOVERABLE = 0x2;

      // A HTTP response code.
      static internal int UPLOAD_RESULT_HTTP_STATUS = 0x8000;
    }

    internal static FfiConfiguration MakeConfig(string dataDir, string packageName,
                                              bool uploadEnabled, int maxEvents)
    {
      glean_enable_logging();

      FfiConfiguration cfg = new FfiConfiguration
      {
        data_dir = dataDir,
        package_name = packageName,
        upload_enabled = uploadEnabled,
        max_events = maxEvents,
        delay_ping_lifetime_io = false
      };

      return cfg;
    }

    internal static bool GleanInitialize(FfiConfiguration cfg)
    {
      return glean_initialize(cfg) != 0;
    }

    internal static void GleanSetUploadEnabled(byte aFlag)
    {
      glean_set_upload_enabled(aFlag);
    }

    internal static byte GleanIsUploadEnabled()
    {
      return glean_is_upload_enabled();
    }

    internal static byte[] EncodeString(string aText)
    {
      if (aText == null || aText.Length == 0)
      {
        return null;
      }
      Encoding utf8 = Encoding.UTF8;
      return utf8.GetBytes(aText);
    }

    internal static byte[] EncodeVectorstring(List<string> aList)
    {
      byte[] values = new byte[0];

      foreach (string s in aList)
      {
        byte[] bytes = EncodeString(s);
        byte[] newArray = new byte[values.Length + bytes.Length];
        values.CopyTo(newArray, 0);
        bytes.CopyTo(newArray, values.Length);
        values = newArray;
      }

      byte[] result = new byte[values.Length + 1];
      values.CopyTo(result, 0);
      result[values.Length] = (byte)0;

      return result;
    }

    internal static UInt64 NewPingType(string aPingName, byte aIncludeClientId, byte aSendIfEmpty,
                                     List<string> aReasonCodes, int aReasonCodesLen)
    {
      string[] reasons = aReasonCodes.ToArray();
      return glean_new_ping_type(aPingName, aIncludeClientId, aSendIfEmpty, reasons, aReasonCodesLen);
    }

    internal static bool GleanSubmitPingByName(string aPingName, string aReason)
    {
      return glean_submit_ping_by_name(aPingName, aReason) != 0;
    }

    internal static bool GleanIsFirstRun()
    {
      return glean_is_first_run() != 0;
    }

    internal static bool GleanOnReadyToSubmitPings()
    {
      return glean_on_ready_to_submit_pings() != 0;
    }

    internal static void GleanClearApplicationLifetimeMetrics()
    {
      glean_clear_application_lifetime_metrics();
    }

    internal static void RegisterPingType(UInt64 aPingTypeHandle)
    {
      glean_register_ping_type(aPingTypeHandle);
    }

    internal static void GleanGetUploadTask(ref FfiPingUploadTask result, byte log_ping)
    {
      glean_get_upload_task(ref result, log_ping);
    }  

    private const string _libFileName = "lib/glean_ffi.dll";
  }

}