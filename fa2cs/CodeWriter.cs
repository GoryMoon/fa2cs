using System;
using System.Collections.Generic;
using System.Linq;
using fa2cs.Helpers;
using fa2cs.Models;

namespace fa2cs
{
    public static class CodeWriter
    {
        public static string Write(IReadOnlyList<Icon> icons, SemanticVersion version)
        {
            Console.Write("Generating C# code...");

            var classTemplate = ResourcesHelper.ReadResourceContent("ClassTemplate.txt");
            var propertyTemplate = ResourcesHelper.ReadResourceContent("PropertyTemplate.txt");

            var properties = icons
                .Select(icon => new
                {
                    icon,
                    property = propertyTemplate.Replace("$link$", icon.Url)
                        .Replace("$name$", icon.Id)
                        .Replace("$code$", icon.Unicode)
                        .Replace("$dotnet_name$", icon.DotNetName)
                        .Replace("$introduced_version$", icon.IntroducedVersion)
                        .Replace("$last_modified_version$", icon.LastModifiedVersion)
                        .Replace("$styles$", icon.StylesSummary)
                        .Replace("$supported_pro_fonts$", string.Join(", ", icon.ProStyles.Select(s => $"FAStyle.{s}")))
                })
                .Select(t =>
                    t.property.Replace("$supported_free_fonts$",
                        t.icon.FreeStyles.Any()
                            ? string.Join(", ", t.icon.FreeStyles.Select(s => $"FAStyle.{s}"))
                            : "FAStyle.Unsupported")
                ).ToList();

            var separator = Environment.NewLine + Environment.NewLine;
            var code = string.Join(separator, properties);

            return classTemplate.Replace("$properties$", code)
                .Replace("$version$", version.ToString());
        }
    }
}