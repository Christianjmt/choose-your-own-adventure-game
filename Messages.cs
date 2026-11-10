using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class Messages
{
    public string CurrentLanguage { get; set; } = "English";
    private Dictionary<string, string> _dictionary = new();
    private Dictionary<string, string> _raceMap = new(StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, string> _occupationMap = new(StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, string> _displayRaceMap = new(StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, string> _displayOccupationMap = new(StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, string> _displayWeaponMap = new(StringComparer.OrdinalIgnoreCase);

    public Messages()
    {
    }

    public void SetCurrentLanguage(string language)
    {
        CurrentLanguage = language;
    }

    public void ReadDictionary()
    {
        string jsonPath = Path.Combine(AppContext.BaseDirectory, "language_data.json");
        if (!File.Exists(jsonPath))
        {
            return;
        }

        string jsonText = File.ReadAllText(jsonPath);
        using JsonDocument doc = JsonDocument.Parse(jsonText);

        if (!doc.RootElement.TryGetProperty(CurrentLanguage, out JsonElement langSection))
        {
            if (!doc.RootElement.TryGetProperty("English", out langSection))
            {
                return;
            }
        }

        _dictionary = LoadMap(langSection, "dictionary");
        _raceMap = LoadMap(langSection, "raceMap", StringComparer.OrdinalIgnoreCase);
        _occupationMap = LoadMap(langSection, "occupationMap", StringComparer.OrdinalIgnoreCase);
        _displayRaceMap = LoadMap(langSection, "displayRaceMap", StringComparer.OrdinalIgnoreCase);
        _displayOccupationMap = LoadMap(langSection, "displayOccupationMap", StringComparer.OrdinalIgnoreCase);
        _displayWeaponMap = LoadMap(langSection, "displayWeaponMap", StringComparer.OrdinalIgnoreCase);
    }

    private static Dictionary<string, string> LoadMap(JsonElement parent, string propertyName, IEqualityComparer<string>? comparer = null)
    {
        var map = comparer != null ? new Dictionary<string, string>(comparer) : new Dictionary<string, string>();
        if (parent.TryGetProperty(propertyName, out JsonElement element))
        {
            foreach (JsonProperty entry in element.EnumerateObject())
            {
                map[entry.Name] = entry.Value.GetString() ?? string.Empty;
            }
        }
        return map;
    }

    public string GetMessage(string key)
    {
        return _dictionary.ContainsKey(key) ? _dictionary[key] : $"[{key}]";
    }

    public bool IsValidRace(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        return _raceMap.ContainsKey(input.Trim());
    }

    public string NormalizeRace(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;
        return _raceMap.TryGetValue(input.Trim(), out string? canonical) ? canonical : input;
    }

    public bool IsValidOccupation(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        return _occupationMap.ContainsKey(input.Trim());
    }

    public string NormalizeOccupation(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;
        return _occupationMap.TryGetValue(input.Trim(), out string? canonical) ? canonical : input;
    }

    public string TranslateRaceForDisplay(string? race)
    {
        if (string.IsNullOrWhiteSpace(race)) return string.Empty;
        return _displayRaceMap.TryGetValue(race.Trim(), out string? translated) ? translated : race;
    }

    public string TranslateOccupationForDisplay(string? occupation)
    {
        if (string.IsNullOrWhiteSpace(occupation)) return string.Empty;
        return _displayOccupationMap.TryGetValue(occupation.Trim(), out string? translated) ? translated : occupation;
    }

    public string TranslateWeaponForDisplay(string? weaponType)
    {
        if (string.IsNullOrWhiteSpace(weaponType)) return string.Empty;
        return _displayWeaponMap.TryGetValue(weaponType.Trim(), out string? translated) ? translated : weaponType;
    }
}
