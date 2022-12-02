using Dapper;
using DatabaseAssessmentTool.Web.Models;
using DatabaseAssessmentTool.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DatabaseAssessmentTool.Web.Pages.Assignments
{
    public class PracticeModel : PageModel
    {
        private readonly IAssessmentToolDbProvider _db;

        public required Assignment Assignment { get; set; }

        public PracticeModel(IAssessmentToolDbProvider db)
        {
            _db = db;
        }

        public async Task<IActionResult> OnGet(int id)
        {
            var assignments = await _db.Connection.QueryAsync<Assignment>("use DbExpert; exec MyAssignments");
            var results = await _db.Connection.QueryAsync<AssignmentResult>("use DbExpert; exec CheckAssignments");
            var assignmentCollection = new AssignmentCollection(assignments, results);
            var assignment = assignmentCollection.GetBy(id);

            if (assignment == null) return NotFound();

            Assignment = assignment;

            return Page();
        }
    }
}
