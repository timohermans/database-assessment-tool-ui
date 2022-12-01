using Dapper;
using DatabaseAssessmentTool.Web.Models;
using DatabaseAssessmentTool.Web.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DatabaseAssessmentTool.Web.Pages.Assignments;

public class IndexModel : PageModel
{
    private readonly IAssessmentToolDbProvider _db;

    public required IEnumerable<Assignment> Assignments { get; set; }

    public IndexModel(IAssessmentToolDbProvider db)
    {
        _db = db;
    }

    public async Task OnGet()
    {
        Assignments = await _db.Connection.QueryAsync<Assignment>("use DbExpert; exec MyAssignments");
        var results = await _db.Connection.QueryAsync<AssignmentResult>("use DbExpert; exec CheckAssignments");
    }
}
