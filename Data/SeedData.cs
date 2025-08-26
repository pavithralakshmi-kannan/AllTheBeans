using System.Globalization;
using System.Text.Json;
using AllTheBeans.Models;
using Microsoft.EntityFrameworkCore;

namespace AllTheBeans.Data;

public static class SeedData
{
    public static async Task EnsureSeedAsync(BeansDb db)
    {
        // if there are any beans already, do nothing
        if (await db.Beans.AnyAsync()) return;

        var jsonPath = Path.Combine(AppContext.BaseDirectory, "Data", "AllTheBeans.json");
        if (!File.Exists(jsonPath)) return;

        var txt = await File.ReadAllTextAsync(jsonPath);
        var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var items = JsonSerializer.Deserialize<List<BeanJson>>(txt, opts);
        if (items == null || items.Count == 0) return;

        var beans = items.Select(x => new Bean
        {
            ExternalId = x._id ?? "",
            Index = x.index,
            IsBOTD = x.isBOTD,
            Name = x.name ?? "",
            Colour = x.colour ?? "",
            Country = x.country ?? "",
            Description = x.description ?? "",
            Image = x.image ?? "",
            Cost = ParseCost(x.cost)
        }).ToList();

        db.Beans.AddRange(beans);
        await db.SaveChangesAsync();
    }

    private static decimal ParseCost(string? costString)
    {
        if (string.IsNullOrWhiteSpace(costString)) return 0m;
        var cleaned = costString.Replace("£", "").Replace("$", "").Trim();
        if (decimal.TryParse(cleaned, NumberStyles.Any, CultureInfo.InvariantCulture, out var c))
            return c;
        return 0m;
    }

    private class BeanJson
    {
        public string? _id { get; set; }
        public int index { get; set; }
        public bool isBOTD { get; set; }
        public string? name { get; set; }
        public string? colour { get; set; }
        public string? country { get; set; }
        public string? description { get; set; }
        public string? image { get; set; }
        public string? cost { get; set; }
    }
}
