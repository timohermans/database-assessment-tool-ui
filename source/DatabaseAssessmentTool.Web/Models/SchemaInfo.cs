using System.Text;
using System.Text.Json;

namespace DatabaseAssessmentTool.Web.Models;

public class SchemaInfo
{
    private readonly List<TableColumnInfo> _items;

    private readonly List<RelationshipInfo>? _relationships;

    public SchemaInfo(IEnumerable<TableColumnInfo> items)
    {
        _items = items.ToList();
    }

    public SchemaInfo(IEnumerable<TableColumnInfo> items, IEnumerable<RelationshipInfo> relationships) : this(items)
    {
        _relationships = relationships.ToList();
    }

    public string ToJson()
    {
        return JsonSerializer.Serialize(_items, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }

    public string ToErdString()
    {
        if (_relationships == null) throw new InvalidOperationException("Retrieve the relationships before trying to generate the ERD");

        var columnsPerTable = _items.GroupBy(c => c.TableName, c => c).ToDictionary(kvp => kvp.Key, kvp => kvp.ToList());

        StringBuilder erd = new StringBuilder();
        erd.AppendLine("erDiagram");

        foreach (var table in columnsPerTable)
        {
            var tableName = table.Key;
            var relationshipsForTable = _relationships.Where(r => r.Table == tableName).ToList();

            foreach (var relationship in relationshipsForTable)
            {
                var columnInfo = table.Value.First(c => c.ColumnName == relationship.Column);
                var nullable = columnInfo.IsNullable ? "o" : "|";
                erd.AppendLine($"\t{tableName} |{nullable}--{nullable}{{ {relationship.ReferencedTable} : {relationship.FkName}");
            }

            erd.AppendLine($"\t{tableName} {{");

            foreach (var column in table.Value)
            {
                erd.AppendLine($"\t\t{column.DataType} {column.ColumnName}");
            }
            erd.AppendLine($"\t}}");
        }

        return erd.ToString();
    }
}
