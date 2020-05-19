
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Glean {
  class Ffi {

    public class FfiConfiguration
    {
      public byte[] data_dir;
      public byte[] package_name;
      public bool upload_enabled;
      public Int32 max_events;
      public bool delay_ping_lifetime_io;
    }

    [DllImport(_libFileName)]
    private static extern void glean_enable_logging();

    [DllImport(_libFileName)]
    private static extern Byte glean_initialize(FfiConfiguration cfg);

    // TODO: Check if aPingName and aReasonCodes work
    // (https://github.com/mozilla/glean/blob/61c542ed822e39b589fcc5b43d824d8bbf5ea693/glean-core/ffi/src/ping_type.rs#L19)
    [DllImport(_libFileName)]
    private static extern UInt64 glean_new_ping_type(Byte[] aPingName, bool aIncludeClientId, bool aSendIfEmpty,
                                                     Byte[] aReasonCodes, Int32 aReasonCodesLen);

    [DllImport(_libFileName)]
    private static extern void glean_register_ping_type(UInt64 aPingTypeHandle);

    public static FfiConfiguration MakeConfig(string dataDir, string packageName,
                                              bool uploadEnabled, int maxEvents)
    {

      glean_enable_logging();

      FfiConfiguration cfg = new FfiConfiguration
      {
        data_dir = EncodeString(dataDir),
        package_name = EncodeString(packageName),
        upload_enabled = uploadEnabled,
        max_events = maxEvents,
        delay_ping_lifetime_io = false
      };

        //    Marshal.Prelink(glean_initialize);
     // Byte b = glean_initialize(cfg);

     // string strDllPath = Path.GetFullPath(_libFileName);
      //  if (File.Exists(strDllPath))
      //  {
      //    try
      //    {
      //      Assembly DLL = Assembly.LoadFile(strDllPath);
      //    } catch (BadImageFormatException e) {
      //        Console.WriteLine("Unable to load {0}.", _libFileName);
      //        Console.WriteLine(e.Message.Substring(0,
      //                          e.Message.IndexOf(".") + 1));
      //    }

      ////    Type classType = DLL.GetType(String.Format("{0}.{1}", strNmSpaceNm, strClassNm));

      //  }
      return cfg;
    }

    public static Byte GleanInitialize(FfiConfiguration cfg)
    {
      return glean_initialize(cfg);
    }

    public static Byte[] EncodeString(String aText)
    {
      if (aText.Length == 0)
      {
        return null;
      }
      Encoding utf8 = Encoding.UTF8;
      return utf8.GetBytes(aText);
    }

    public static Byte[] EncodeVectorString(List<String> aList)
    {
      Byte[] values = new Byte[0];

      foreach (String s in aList)
      {
        Byte[] bytes = EncodeString(s);
        Byte[] newArray = new Byte[values.Length + bytes.Length];
        values.CopyTo(newArray, 0);
        bytes.CopyTo(newArray, values.Length);
        values = newArray;
      }

      Byte[] result = new Byte[values.Length + 1];
      values.CopyTo(result, 0);
      result[values.Length] = (Byte)0;

      return result;
    }

    public static UInt64 NewPingType(Byte[] aPingName, bool aIncludeClientId, bool aSendIfEmpty,
                              Byte[] aReasonCodes, Int32 aReasonCodesLen)
    {
      return glean_new_ping_type(aPingName, aIncludeClientId, aSendIfEmpty, aReasonCodes, aReasonCodesLen);
    }

    public static void RegisterPingType(UInt64 aPingTypeHandle)
    {
      glean_register_ping_type(aPingTypeHandle);
    }

    private const string _libFileName = "glean_ffi.dll";
  }

}