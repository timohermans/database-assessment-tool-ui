using Dapper;
using DatabaseAssessmentTool.Web.Models;
using DatabaseAssessmentTool.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace DatabaseAssessmentTool.Web.Pages.Assignments
{
    public class PracticeModel : PageModel
    {
        private readonly IAssessmentToolDbProvider _dbProvider;

        public required Assignment Assignment { get; set; }
        public required SchemaInfo SchemaInfo { get; set; }
        public required IEnumerable<dynamic>? QueryResult { get; set; }
        public string? QueryError { get; set; }

        [BindProperty]
        public bool IsFinalQuery { get; set; }

        [BindProperty]
        public string Query { get; set; }

        public PracticeModel(IAssessmentToolDbProvider provider)
        {
            _dbProvider = provider;
        }

        public async Task<IActionResult> OnGetAsync(int id)
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
            var assignments = await db.QueryAsync<Assignment>("exec MyAssignments");
            var assignmentCollection = new AssignmentCollection(assignments);
            await UpdateAssignmentResults(db, assignmentCollection);
            var assignment = assignmentCollection.GetBy(id);

            if (assignment == null) return null;

            return assignment;
        }

        private async Task UpdateAssignmentResults(SqlConnection db, AssignmentCollection collection)
        {
            var results = await db.QueryAsync<AssignmentResult>("exec CheckAssignments");
            collection.UpdateAssignmentsWithResults(results);
        }

        private async Task<SchemaInfo> GetSchemaInfo(string databaseName)
        {
            var query = """
select 
  TABLE_NAME as [TableName], 
  COLUMN_NAME as [ColumnName], 
  DATA_TYPE as [DataType]
from INFORMATION_SCHEMA.COLUMNS
where TABLE_NAME <> 'sysdiagrams'
  and TABLE_NAME not like 'v[_]%'
""";

            using var db = _dbProvider.Provide(databaseName);
            var tableColumnInfoItems = await db.QueryAsync<TableColumnInfo>(query);
            return new SchemaInfo(tableColumnInfoItems);
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var getResult = await OnGetAsync(id);

            if (getResult is not PageResult) return getResult;

            using var db = _dbProvider.Provide(Assignment.DatabaseName);

            try
            {
                var query = Assignment.Comments.Replace("<your query>", Query.Replace(";", ""));
                await db.ExecuteAsync(query);
                QueryResult = await db.QueryAsync(Query);
                Assignment = await GetAssignmentAsync(id) ?? throw new InvalidOperationException("Something in the database must have happened");
            } catch (SqlException error)
            {
                QueryError = error.Message;
            }

            return Page();
        }

    }
}
