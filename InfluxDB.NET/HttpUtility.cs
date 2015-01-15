﻿using System;

namespace InfluxDB.Net
{
    internal static class HttpUtility
    {
        public static string UrlEncode(string parameter)
        {
            return Uri.EscapeUriString(parameter);
        }
    }
}