using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GlobalPanda
{
    internal class TimeZoneInformation
    {
        private TimeZoneInformation()
        {
        }

        private static TimeZoneInformation[] s_zones = null;
        private static readonly object s_lockZones = new object();

        /// <summary>
        /// Get the currently selected time zone
        /// </summary>

        public static TimeZoneInformation CurrentTimeZone
        {
            get
            {
                // The currently selected time zone information can
                // be retrieved using the Win32 GetTimeZoneInformation call,
                // but it only gives us names, offsets and dates - crucially,
                // not the Index.

                TIME_ZONE_INFORMATION tziNative;
                TimeZoneInformation[] zones = EnumZones();

                NativeMethods.GetTimeZoneInformation(out tziNative);

                // Getting the identity is tricky; the best we can do
                // is a match on the properties.

                for (int idx = 0; idx < zones.Length; ++idx)
                {
                    if (zones[idx].m_tzi.bias == tziNative.Bias &&
                         zones[idx].m_tzi.daylightBias == tziNative.DaylightBias &&
                         zones[idx].m_tzi.standardBias == tziNative.StandardBias &&
                         zones[idx].m_standardName == tziNative.StandardName &&
                         zones[idx].m_daylightName == tziNative.DaylightName)
                    {
                        return zones[idx];
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Get a TimeZoneInformation for a supplied index.
        /// </summary>
        /// <param name="index">The time zone to find.</param>
        /// <returns>The corresponding TimeZoneInformation.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown
        ///      if the index is not found.</exception>

        public static TimeZoneInformation FromIndex(int index)
        {
            TimeZoneInformation[] zones = EnumZones();

            for (int i = 0; i < zones.Length; ++i)
            {
                if (zones[i].Index == index)
                    return zones[i];
            }

            throw new ArgumentOutOfRangeException("index",
                         index, "Unknown time zone index");
        }

        /// <summary>
        /// Enumerate the available time zones
        /// </summary>
        /// <returns>The list of known time zones</returns>

        public static TimeZoneInformation[] EnumZones()
        {
            if (s_zones == null)
            {
                lock (s_lockZones)
                {
                    if (s_zones == null)
                    {
                        ArrayList zones = new ArrayList();

                        using (RegistryKey key =
                           Registry.LocalMachine.OpenSubKey(
                           @"SOFTWARE\Microsoft\Windows NT" +
                           @"\CurrentVersion\Time Zones"))
                        {
                            string[] zoneNames = key.GetSubKeyNames();

                            foreach (string zoneName in zoneNames)
                            {
                                using (RegistryKey subKey = key.OpenSubKey(zoneName))
                                {
                                    TimeZoneInformation tzi = new TimeZoneInformation();
                                    tzi.m_name = zoneName;
                                    tzi.m_displayName = (string)subKey.GetValue("Display");
                                    tzi.m_standardName = (string)subKey.GetValue("Std");
                                    tzi.m_daylightName = (string)subKey.GetValue("Dlt");
                                    if (subKey.GetValue("Index") != null)
                                    {
                                        // Pre-Vista
                                        tzi.m_index = (int)(subKey.GetValue("Index"));
                                    }
                                    else
                                    {
                                        // Vista
                                        tzi.m_index = int.Parse(((string)subKey.GetValue("MUI_Display")).Split(',')[1]) * -1;
                                    }

                                    tzi.InitTzi((byte[])subKey.GetValue("Tzi"));

                                    zones.Add(tzi);
                                }
                            }
                        }

                        s_zones = new TimeZoneInformation[zones.Count];

                        zones.CopyTo(s_zones);
                    }
                }
            }

            return s_zones;
        }

        /// <summary>
        /// The zone's name.
        /// </summary>

        public string Name
        {
            get { return m_name; }
        }

        /// <summary>
        /// The zone's display name, e.g. '(GMT) Greenwich Mean
        ///            Time : Dublin, Edinburgh, Lisbon, London'.
        /// </summary>

        public string DisplayName
        {
            get { return m_displayName; }
        }

        /// <summary>
        /// The zone's index. No obvious pattern.
        /// </summary>

        public int Index
        {
            get { return m_index; }
        }

        /// <summary>
        /// The zone's name during 'standard' time (not daylight savings).
        /// </summary>

        public string StandardName
        {
            get { return m_standardName; }
        }

        /// <summary>
        /// The zone's name during daylight savings time.
        /// </summary>

        public string DaylightName
        {
            get { return m_daylightName; }
        }

        public override string ToString()
        {
            return m_displayName;
        }

        /// <summary>
        /// The standard Windows SYSTEMTIME structure.
        /// </summary>

        [StructLayout(LayoutKind.Sequential)]
        private struct SYSTEMTIME
        {
            public UInt16 wYear;
            public UInt16 wMonth;
            public UInt16 wDayOfWeek;
            public UInt16 wDay;
            public UInt16 wHour;
            public UInt16 wMinute;
            public UInt16 wSecond;
            public UInt16 wMilliseconds;
        }

        // FILETIME is already declared in System.Runtime.InteropServices.

        /// <summary>
        /// The layout of the Tzi value in the registry.
        /// </summary>

        [StructLayout(LayoutKind.Sequential)]
        private struct TZI
        {
            public int bias;
            public int standardBias;
            public int daylightBias;
            public SYSTEMTIME standardDate;
            public SYSTEMTIME daylightDate;
        }

        /// <summary>
        /// The standard Win32 TIME_ZONE_INFORMATION structure.
        /// Thanks to www.pinvoke.net.
        /// </summary>

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct TIME_ZONE_INFORMATION
        {
            [MarshalAs(UnmanagedType.I4)]
            public Int32 Bias;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string StandardName;

            public SYSTEMTIME StandardDate;

            [MarshalAs(UnmanagedType.I4)]
            public Int32 StandardBias;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DaylightName;

            public SYSTEMTIME DaylightDate;

            [MarshalAs(UnmanagedType.I4)]
            public Int32 DaylightBias;
        }

        /// <summary>
        /// A container for P/Invoke declarations.
        /// </summary>

        private struct NativeMethods
        {
            private const string KERNEL32 = "kernel32.dll";

            [DllImport(KERNEL32)]
            public static extern uint
                   GetTimeZoneInformation(out TIME_ZONE_INFORMATION
                   lpTimeZoneInformation);

            [DllImport(KERNEL32)]
            public static extern bool SystemTimeToTzSpecificLocalTime(
                [In] ref TIME_ZONE_INFORMATION lpTimeZone,
                [In] ref SYSTEMTIME lpUniversalTime,
                out SYSTEMTIME lpLocalTime);

            [DllImport(KERNEL32)]
            public static extern bool SystemTimeToFileTime(
                [In] ref SYSTEMTIME lpSystemTime,
                out FILETIME lpFileTime);

            [DllImport(KERNEL32)]
            public static extern bool FileTimeToSystemTime(
                [In] ref FILETIME lpFileTime,
                out SYSTEMTIME lpSystemTime);

            /// <summary>
            /// Convert a local time to UTC, using
            /// the supplied time zone information.
            /// Windows XP and Server 2003 and later only.
            /// </summary>

            /// <param name="lpTimeZone">The time zone to use.</param>
            /// <param name="lpLocalTime">The local time to convert.</param>
            /// <param name="lpUniversalTime">The resultant time in UTC.</param>
            /// <returns>true if successful, false otherwise.</returns>

            [DllImport(KERNEL32)]
            public static extern bool TzSpecificLocalTimeToSystemTime(
                [In] ref TIME_ZONE_INFORMATION lpTimeZone,
                [In] ref SYSTEMTIME lpLocalTime,
                out SYSTEMTIME lpUniversalTime);
        }

        /// <summary>
        /// Initialise the m_tzi member.
        /// </summary>
        /// <param name="info">The Tzi data from the registry.</param>

        private void InitTzi(byte[] info)
        {
            if (info.Length != Marshal.SizeOf(m_tzi))
            {
                throw new ArgumentException("Information size is incorrect",
                                                                     "info");
            }

            // Could have sworn there's a Marshal operation to pack bytes into
            // a structure, but I can't see it. Do it manually.

            GCHandle h = GCHandle.Alloc(info, GCHandleType.Pinned);

            try
            {
                m_tzi = (TZI)Marshal.PtrToStructure(h.AddrOfPinnedObject(),
                                                              typeof(TZI));
            }
            finally
            {
                h.Free();
            }
        }

        /// <summary>
        /// The offset from UTC. Local = UTC + Bias.
        /// </summary>

        public int Bias
        {
            // Biases in the registry are defined as UTC = local + bias
            // We return as Local = UTC + bias

            get { return -m_tzi.bias; }
        }

        /// <summary>
        /// The offset from UTC during standard time.
        /// </summary>

        public int StandardBias
        {
            get { return -(m_tzi.bias + m_tzi.standardBias); }
        }

        /// <summary>
        /// The offset from UTC during daylight time.
        /// </summary>

        public int DaylightBias
        {
            get { return -(m_tzi.bias + m_tzi.daylightBias); }
        }

        private TIME_ZONE_INFORMATION TziNative()
        {
            TIME_ZONE_INFORMATION tziNative = new TIME_ZONE_INFORMATION();

            tziNative.Bias = m_tzi.bias;
            tziNative.StandardDate = m_tzi.standardDate;
            tziNative.StandardBias = m_tzi.standardBias;
            tziNative.DaylightDate = m_tzi.daylightDate;
            tziNative.DaylightBias = m_tzi.daylightBias;

            return tziNative;
        }

        /// <summary>
        /// Convert a time interpreted as UTC to a time in this time zone.
        /// </summary>
        /// <param name="utc">The UTC time to convert.</param>
        /// <returns>The corresponding local time in this zone.</returns>

        public DateTime FromUniversalTime(DateTime utc)
        {
            // Convert to SYSTEMTIME

            SYSTEMTIME stUTC = DateTimeToSystemTime(utc);

            // Set up the TIME_ZONE_INFORMATION

            TIME_ZONE_INFORMATION tziNative = TziNative();

            SYSTEMTIME stLocal;

            NativeMethods.SystemTimeToTzSpecificLocalTime(ref
                          tziNative, ref stUTC, out stLocal);

            // Convert back to DateTime

            return SystemTimeToDateTime(ref stLocal);
        }

        /// <summary>
        /// Convert a time from UTC to the time zone with the supplied index.
        /// </summary>
        /// <param name="index">The time zone index.</param>
        /// <param name="utc">The time to convert.</param>
        /// <returns>The converted time.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if
        ///      the index is not found.</exception>

        public static DateTime FromUniversalTime(int index, DateTime utc)
        {
            TimeZoneInformation tzi = FromIndex(index);

            return tzi.FromUniversalTime(utc);
        }

        /// <summary>
        /// Convert a time interpreted as a local time in this zone to the equivalent UTC.
        /// Note that there may be different possible interpretations
        /// at the daylight time boundaries.
        /// </summary>
        /// <param name="local">The local time to convert.</param>
        /// <returns>The corresponding UTC.</returns>
        /// <exception cref="NotSupportedException">Thrown if
        ///     the method failed due to missing platform support.</exception>

        public DateTime ToUniversalTime(DateTime local)
        {
            SYSTEMTIME stLocal = DateTimeToSystemTime(local);

            TIME_ZONE_INFORMATION tziNative = TziNative();

            SYSTEMTIME stUTC;

            try
            {
                NativeMethods.TzSpecificLocalTimeToSystemTime(ref
                                tziNative, ref stLocal, out stUTC);

                return SystemTimeToDateTime(ref stUTC);
            }
            catch (EntryPointNotFoundException e)
            {
                throw new NotSupportedException("This method" +
                      " is not supported on this operating system", e);
            }
        }

        /// <summary>
        /// Convert a time from the time zone with the supplied index to UTC.
        /// </summary>

        /// <param name="index">The time zone index.</param>
        /// <param name="utc">The time to convert.</param>
        /// <returns>The converted time.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if
        ///               the index is not found.</exception>
        /// <exception cref="NotSupportedException">Thrown if the
        ///     method failed due to missing platform support.</exception>

        public static DateTime ToUniversalTime(int index, DateTime local)
        {
            TimeZoneInformation tzi = FromIndex(index);

            return tzi.ToUniversalTime(local);
        }

        private static SYSTEMTIME DateTimeToSystemTime(DateTime dt)
        {
            SYSTEMTIME st;
            FILETIME ft = new FILETIME();

            ft.dwHighDateTime = (int)(dt.Ticks >> 32);
            ft.dwLowDateTime = (int)(dt.Ticks & 0xFFFFFFFFL);

            NativeMethods.FileTimeToSystemTime(ref ft, out st);

            return st;
        }

        private static DateTime SystemTimeToDateTime(ref SYSTEMTIME st)
        {
            FILETIME ft = new FILETIME();

            NativeMethods.SystemTimeToFileTime(ref st, out ft);

            DateTime dt = new DateTime((((long)ft.dwHighDateTime)
                           << 32) | (uint)ft.dwLowDateTime);

            return dt;
        }

        private TZI m_tzi;
        private string m_name;
        private string m_displayName;
        private int m_index;
        private string m_standardName;
        private string m_daylightName;
    }

    public class TZConvert
    {
        static private float ExtractGMTOffset(string s)
        {
            // (GMT)
            // (GMT+10:30) xxx

            string tmp = s.Substring(4);
            int i = tmp.IndexOf(")");
            tmp = tmp.Substring(0, i);
            tmp = tmp.Replace(":", ".");
            if (tmp == "")
                return (0.0F);
            return (System.Single.Parse(tmp));
        }

        public static string GetTZName(int timezone)
        {
            TimeZoneInformation[] zones = TimeZoneInformation.EnumZones();
            string name = "Unknown";
            for (int i = 0; i < zones.Length; i++)
            {
                if (timezone == zones[i].Index)
                {
                    name = zones[i].DisplayName;
                }
            }
            return name;
        }

        public static void GetTZList(System.Collections.ArrayList Indexes,
                                       System.Collections.ArrayList Names)
        {
            TimeZoneInformation[] zones = TimeZoneInformation.EnumZones();
            float[] GMTOffsets = new float[zones.Length];
            for (int i = 0; i < zones.Length; i++)
                GMTOffsets[i] = ExtractGMTOffset(zones[i].DisplayName);
            Array.Sort(GMTOffsets, zones);
            for (int i = 0; i < zones.Length; i++)
            {
                Indexes.Add(zones[i].Index);
                Names.Add(zones[i].DisplayName);
            }
        }

        public static Dictionary<int, string> GetTZList()
        {
            Dictionary<int, string> tzis = new Dictionary<int, string>();
            TimeZoneInformation[] zones = TimeZoneInformation.EnumZones();
            float[] GMTOffsets = new float[zones.Length];
            for (int i = 0; i < zones.Length; i++)
                GMTOffsets[i] = ExtractGMTOffset(zones[i].DisplayName);
            Array.Sort(GMTOffsets, zones);
            for (int i = 0; i < zones.Length; i++)
            {
                tzis.Add(zones[i].Index, zones[i].DisplayName);
            }
            return tzis;
        }

        public static void GetSpecificTZList(string[] Indexes, string[] Names)
        {
            TimeZoneInformation[] zones = TimeZoneInformation.EnumZones();
            int[] iIndexes = new int[Indexes.Length];
            for (int i = 0; i < Indexes.Length; i++)
                iIndexes[i] = System.Int32.Parse(Indexes[i]);

            int j = 0;
            for (int i = 0; i < zones.Length; i++)
            {
                j = Array.IndexOf(iIndexes, zones[i].Index);
                if (j >= 0)
                    Names[j] = zones[i].DisplayName;
            }
        }

        private static System.DateTime MakeDateHelper(int Year,
                         int Month, int Day, int Hour, int Min)
        {
            System.DateTime dt = new System.DateTime(Year,
                             Month, Day, Hour, Min, 0, 0);
            return (dt);
        }

        public static System.DateTime MakeDateTime(int Index, string s)
        {
            // 012345678911234567
            // 2000/01/01 03:23pm

            if (!(s.Length == 16 || s.Length == 18))
                throw (new System.FormatException());
            if (s[4] != '/') throw (new System.FormatException());
            if (s[7] != '/') throw (new System.FormatException());
            if (s[10] != ' ') throw (new System.FormatException());
            if (s[13] != ':') throw (new System.FormatException());
            int Year = System.Int32.Parse(s.Substring(0, 4));
            int Month = System.Int32.Parse(s.Substring(5, 2));
            int Day = System.Int32.Parse(s.Substring(8, 2));
            int Hour = System.Int32.Parse(s.Substring(11, 2));
            int Minute = System.Int32.Parse(s.Substring(14, 2));
            bool IsPM = false;
            if (s.Length == 18)
                IsPM = (s.Substring(16).ToLower() == "pm") ? true : false;

            if (IsPM)
            {
                if (Hour != 12)
                    Hour += 12;
            }
            else
            {
                if (Hour == 12)
                    Hour -= 12;
            }
            System.DateTime dt = new System.DateTime(Year,
                                     Month, Day, Hour, Minute, 0, 0);
            if (Index != -1)
                return (TimeZoneInformation.ToUniversalTime(Index, dt));
            return (dt);
        }

        public static string DisplayDateTime(int Index,
                      System.DateTime dt, bool IncludeDayName)
        {
            if (Index != -1)
                dt = TimeZoneInformation.FromUniversalTime(Index, dt);
            string t = dt.ToString("yyyy/MM/dd\\ HH:mmtt\\ dddd",
                       System.Globalization.DateTimeFormatInfo.InvariantInfo);
            System.Text.StringBuilder s = new
                        System.Text.StringBuilder(t.Substring(0, 16));
            string DayName = t.Substring(17, 3);

            if (IncludeDayName)
            {
                s.Append(" ");
                s.Append(DayName);
            }
            return (s.ToString());
        }

        public static string DisplayDateTime(int Index, System.DateTime dt)
        {
            return (DisplayDateTime(Index, dt, false));
        }

        //--------------- updates start here ---------------

        public static string DisplayLocalTimeByBrowserHelper()
        {
            return (
                @"<script>
                function FormatLocalTime(dd, IncludeDayName)
                {
                    function TwoDigits(s)
                    {
                        var ss = s.toString();
                        if(ss.length == 1)
                            return('0'+ss);
                        return(ss);
                    }

                    var d=new Date(dd);
                    var Year = d.getFullYear().toString();
                    var Month = TwoDigits(d.getMonth());
                    var Day = TwoDigits(d.getDate());
                    var Hours = d.getHours();
                    var Minutes = TwoDigits(d.getMinutes());
                    var AmPm = 'am';
                    if(Hours == 12)
                    {
                        AmPm = 'pm';
                    }
                    if(Hours == 0)
                    {
                        Hours = 12;
                    }
                    if(Hours > 12)
                    {
                        Hours -= 12;
                        AmPm = 'pm';
                    }
                    return(Year+'/'+Month+'/'+Day+' '+
                           TwoDigits(Hours)+':'+Minutes+AmPm);
                }
                </script>
                ");
        }

        public static string DisplayLocalTimeByBrowser(System.DateTime dt,
                                                       bool IncludeDayName)
        {
            return ("<script>document.write(FormatLocalTime("
                    + "Date.UTC("
                    + dt.Year.ToString() + ","
                    + dt.Month.ToString() + ","
                    + dt.Day.ToString() + ","
                    + dt.Hour.ToString() + ","
                    + dt.Minute.ToString() + ",0)"
                    + ", "
                    + (IncludeDayName == true ? "true" : "false")
                    + "));</script>"
            );
        }

        //--------------- updates end here ---------------

        public static string DisplayXmlDateTime(System.DateTime dt)
        {
            // 2004-01-17T12:46:40.837

            return (dt.ToString("yyyy-MM-dd\\Thh:mm:ss.fff",
              System.Globalization.DateTimeFormatInfo.InvariantInfo));
        }

        public static void Main(string[] args)
        {
            ArrayList Indexes = new ArrayList();
            ArrayList Names = new ArrayList();
            GetTZList(Indexes, Names);
            for (int i = 0; i < Indexes.Count; i++)
            {
                System.Console.WriteLine(
                    Indexes[i] + " " +
                    Names[i]);
            }

            System.DateTime dt = MakeDateTime(35, "2003/03/02 03:43pm");
            System.Console.WriteLine(TZConvert.DisplayDateTime(35, dt));
            string s = DisplayLocalTimeByBrowserHelper() +
                       DisplayLocalTimeByBrowser(dt, true);
            System.Console.WriteLine(s);
        }
    }
}