﻿using Microsoft.VisualStudio.ApplicationInsights;
using System.Diagnostics;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace CodeNav.Helpers;

public static class LogHelper
{
    private static TelemetryClient? _client;
    private const string InstrumentationKey = "0913ac4a-1127-4d28-91cf-07673e70200f";

    public static void GetClient()
    {
        _client = new TelemetryClient();
        _client.Context.Session.Id = Guid.NewGuid().ToString();
        _client.InstrumentationKey = InstrumentationKey;
        _client.Context.Component.Version = GetExecutingAssemblyVersion().ToString();
        _client.Context.User.Id = GetUserId();
    }

    public static void Log(string message, Exception? exception = null, 
        object? additional = null, string language = "")
    {
        if (_client == null)
        {
            GetClient();
        }

        if (_client == null)
        {
            return;
        }

        var properties = new Dictionary<string, string>
        {
            { "version", GetExecutingAssemblyVersion().ToString() },
            { "message", JsonSerializer.Serialize(message) },
            { "language", language },
            { "additional", JsonSerializer.Serialize(additional) }
        };

        if (exception == null)
        {
            _client.TrackEvent(message, properties);
        }
        else
        {
            _client.TrackException(exception, properties);
        }
    }

    private static Version GetExecutingAssemblyVersion()
    {
        var ver = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);

        // read what's defined in [assembly: AssemblyFileVersion("1.2.3.4")]
        return new Version(ver.ProductMajorPart, ver.ProductMinorPart, ver.ProductBuildPart, ver.ProductPrivatePart);
    }

    private static string GetUserId()
    {
        var enc = Encoding.UTF8.GetBytes(Environment.UserName + Environment.MachineName);
        var hash = MD5.Create().ComputeHash(enc);
        return Convert.ToBase64String(hash);
    }
}
