using Dapper;
using DatabaseAssessmentTool.Web.Models;
using DatabaseAssessmentTool.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace DatabaseAssessmentTool.Web.Pages.Assignments
{
    public class PracticeModel : PageModel
    {
        private readonly IAssessmentToolDbProvider _dbProvider;

        public required Assignment Assignment { get; set; }
        public required SchemaInfo SchemaInfo { get; set; }

        public PracticeModel(IAssessmentToolDbProvider provider)
        {
            _dbProvider = provider;
        }

        public async Task<IActionResult> OnGet(int id)
        {
            var assignment = await GetAssignmentAsync(id);
            if (assignment == null) return NotFound();
            Assignment = assignment;
            SchemaInfo = await GetSchemaInfo(assignment.DatabaseName);

            return Page();
        }

        private async Task<Assignment?> GetAssignmentAsync(int id)
        {
            using var db = _dbProvider.Provide();
            var assignments = await db.QueryAsync<Assignment>("use DbExpert; exec MyAssignments");
            var results = await db.QueryAsync<AssignmentResult>("use DbExpert; exec CheckAssignments");
            var assignmentCollection = new AssignmentCollection(assignments, results);
            var assignment = assignmentCollection.GetBy(id);

            if (assignment == null) return null;

            return assignment;
        }

        private async Task<SchemaInfo> GetSchemaInfo(string databaseName)
        {
            var query = $"""
select 
  TABLE_NAME as [TableName], 
  COLUMN_NAME as [ColumnName], 
  DATA_TYPE as [DataType]
from {databaseName}.INFORMATION_SCHEMA.COLUMNS
where TABLE_NAME <> 'sysdiagrams'
  and TABLE_NAME not like 'v_%'
""";
            using var db = _dbProvider.Provide();
            var tableColumnInfoItems = await db.QueryAsync<TableColumnInfo>(query);
            return new SchemaInfo(tableColumnInfoItems);
        }

    }
}
