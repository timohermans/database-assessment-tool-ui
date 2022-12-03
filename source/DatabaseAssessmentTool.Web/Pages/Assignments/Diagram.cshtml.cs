using Dapper;
using DatabaseAssessmentTool.Web.Models;
using DatabaseAssessmentTool.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DatabaseAssessmentTool.Web.Pages.Assignments
{
    public class DiagramModel : PageModel
    {
        private readonly IAssessmentToolDbProvider _provider;

        public required string Database { get; set; }
        public required SchemaInfo Schema { get; set; }

        public DiagramModel(IAssessmentToolDbProvider provider)
        {
            _provider = provider;
        }

        public async Task<IActionResult> OnGet(string database)
        {
            if (database == null) return NotFound();

            Database = database;

            Schema = await GetSchemaInfoAsync(database);

            return Page();
        }

        private async Task<SchemaInfo> GetSchemaInfoAsync(string databaseName)
        {
            var query = """
select 
  TABLE_NAME as [TableName], 
  COLUMN_NAME as [ColumnName], 
  DATA_TYPE as [DataType],
  IS_NULLABLE as [IsNullableText]
from INFORMATION_SCHEMA.COLUMNS
where TABLE_NAME <> 'sysdiagrams'
  and TABLE_NAME not like 'v[_]%'
""";
            var queryRelationships = """
SELECT  obj.name AS FkName,
    tab1.name AS [Table],
    col1.name AS [Column],
    tab2.name AS [ReferencedTable],
    col2.name AS [ReferencedColumn]
FROM sys.foreign_key_columns fkc
INNER JOIN sys.objects obj
    ON obj.object_id = fkc.constraint_object_id
INNER JOIN sys.tables tab1
    ON tab1.object_id = fkc.parent_object_id
INNER JOIN sys.columns col1
    ON col1.column_id = parent_column_id AND col1.object_id = tab1.object_id
INNER JOIN sys.tables tab2
    ON tab2.object_id = fkc.referenced_object_id
INNER JOIN sys.columns col2
    ON col2.column_id = referenced_column_id AND col2.object_id = tab2.object_id
""";

            using var db = _provider.Provide(databaseName);
            var tableColumnInfoItems = await db.QueryAsync<TableColumnInfo>(query);
            var relationshipInfoItems = await db.QueryAsync<RelationshipInfo>(queryRelationships);
            return new SchemaInfo(tableColumnInfoItems, relationshipInfoItems);
        }
    }
}
