using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace PersonalFinanceApp.Infrastructure.Persistence.Seed;

public static class SeedJsonLoader
{
    public static List<T> Load<T>(string embeddedFileName)
    {
        var assembly = typeof(SeedJsonLoader).Assembly;
        var resourceName = assembly.GetManifestResourceNames()
            .Single(n => n.EndsWith(embeddedFileName, StringComparison.OrdinalIgnoreCase));

        using var stream = assembly.GetManifestResourceStream(resourceName)!;
        return JsonSerializer.Deserialize<List<T>>(stream,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? [];
    }
}

public record AccountTypeTranslationSeedRow(string AccountTypeCode, string LanguageCode, string Translation);
