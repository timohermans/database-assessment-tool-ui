using Dapper;
using DatabaseAssessmentTool.Web.Models;
using DatabaseAssessmentTool.Web.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DatabaseAssessmentTool.Web.Pages.Assignments;

public class IndexModel : PageModel
{
    private readonly IAssessmentToolDbProvider _provider;

    public required AssignmentCollection Assignments { get; set; }

    public IndexModel(IAssessmentToolDbProvider db)
    {
        _provider = db;
    }

    public async Task OnGet()
    {
        using var db = _provider.Provide();
        var assignments = await db.QueryAsync<Assignment>("use DbExpert; exec MyAssignments");
        var results = await db.QueryAsync<AssignmentResult>("use DbExpert; exec CheckAssignments");
        Assignments = new AssignmentCollection(assignments, results);
    }
}
