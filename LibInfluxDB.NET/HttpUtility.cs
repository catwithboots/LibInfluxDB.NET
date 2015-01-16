using System;

namespace LibInfluxDB.Net
{
    internal static class HttpUtility
    {
        public static string UrlEncode(string parameter)
        {
            return Uri.EscapeUriString(parameter);
        }
    }
}