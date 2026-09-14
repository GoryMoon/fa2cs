using System;
using fa2cs.Helpers;

namespace fa2cs;

public static class ReadmeWriter
{
    public static string Write(string fontAwesomeVersion)
    {
        Console.Write("Generating repository readme...");

        var readmeTemplate = ResourcesHelper.ReadResourceContent("Readme.txt");

        var now = DateTimeOffset.UtcNow;

        return readmeTemplate.Replace("$latest_version$", fontAwesomeVersion)
            .Replace("$exported_date_time$", now.ToString("F"))
            .Replace("$exported_timezone$", "UTC");
    }
}