using System.Text.Json;

namespace DatabaseAssessmentTool.Web.Models;

public class SchemaInfo
{
    private readonly List<TableColumnInfo> _items;

    public SchemaInfo(IEnumerable<TableColumnInfo> items)
    {
        _items = items.ToList();
    }

    public string ToJson()
    {
        return JsonSerializer.Serialize(_items, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }
}
