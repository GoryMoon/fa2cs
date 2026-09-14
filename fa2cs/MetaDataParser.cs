using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using fa2cs.Models;
using Newtonsoft.Json.Linq;

namespace fa2cs;

public static class MetaDataParser
{
    public static IReadOnlyList<Icon> Parse(string metaDataFilePath, out SemanticVersion latestVersion)
    {
        if (string.IsNullOrEmpty(metaDataFilePath))
        {
            throw new ArgumentException($"'{nameof(metaDataFilePath)}' cannot be null or empty.", nameof(metaDataFilePath));
        }

        if (!File.Exists(metaDataFilePath))
        {
            throw new FileNotFoundException($"Unable to locate the 'icons.json' meta data file at the path: {metaDataFilePath}");
        }

        latestVersion = null;
        Console.WriteLine($"Parsing the FontAwesome icons.json meta data file: '{metaDataFilePath}'");

        var data = JObject.Parse(File.ReadAllText(metaDataFilePath));

        var icons = new List<Icon>();

        Console.WriteLine($" -> Found {data.Count} icons to process...");

        var distinctVersions = new HashSet<string>();

        var itemNumber = 0;
        foreach (var element in data)
        {
            itemNumber++;
            var icon = ParseIcon(element, itemNumber, data.Count);

            if (icon != null)
            {
                icons.Add(icon);

                foreach (var version in icon.Versions)
                {
                    distinctVersions.Add(version);
                }
            }
        }

        var versions = distinctVersions.Select(v => new SemanticVersion(v)).ToList();

        versions.Sort();

        latestVersion = versions.Last();

        return icons;
    }

    private const string ChangesKey = "changes";
    private const string UnicodeKey = "unicode";
    private const string ProFontFamiliesKey = "styles";
    private const string FreeFontFamiliesKey = "free";

    private static Icon ParseIcon(KeyValuePair<string, JToken> element, int itemNumber, int total)
    {
        var id = element.Key;

        Console.WriteLine($" --> Processing '{id}' ({itemNumber}/{total}).");

        var jsonToken = element.Value;
        var unicode = jsonToken.Value<string>(UnicodeKey);

        // Ensure that the 
        if (unicode.Length < 4)
        {
            unicode = new string('0', 4 - unicode.Length) + unicode;
        }

        var versions = jsonToken.Value<JArray>(ChangesKey)
            .ToObject<List<string>>();

        var freeFontFamilies = jsonToken.Value<JArray>(FreeFontFamiliesKey)
            .ToObject<List<string>>()
            .Select(v => Enum.Parse<Style>(v, ignoreCase: true))
            .ToList();

        var proFontFamilies = jsonToken.Value<JArray>(ProFontFamiliesKey)
            .ToObject<List<string>>()
            .Select(v => Enum.Parse<Style>(v, ignoreCase: true))
            .ToList();

        return new Icon(id, unicode, versions, freeFontFamilies, proFontFamilies);
    }
}