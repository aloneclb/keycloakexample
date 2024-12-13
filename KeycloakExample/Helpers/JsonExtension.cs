using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace KeycloakExample.Helpers;

public static class JsonExtension
{

    public static readonly JsonSerializerOptions DefaultSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
        AllowTrailingCommas = true,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
    };

    public static string ToJson<T>(this T obj)
    {
        return JsonSerializer.Serialize(obj, DefaultSerializerOptions);
    }

    public static T? FromJson<T>(this string json)
    {
        if (string.IsNullOrEmpty(json))
        {
            return default;
        }

        try
        {
            return JsonSerializer.Deserialize<T>(json, DefaultSerializerOptions);
        }
        catch
        {
            return default;
        }
    }
}
