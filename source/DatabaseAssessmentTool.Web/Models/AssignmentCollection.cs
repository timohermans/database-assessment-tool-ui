namespace DatabaseAssessmentTool.Web.Models;

public class AssignmentCollection
{
    public List<Assignment> Items { get; private set; }

    public AssignmentCollection(IEnumerable<Assignment> assignments, IEnumerable<AssignmentResult> results)
    {
        Items = assignments.ToList();
        UpdateAssignmentsWithResults(results);
    }

    private void UpdateAssignmentsWithResults(IEnumerable<AssignmentResult> results)
    {
        foreach (var assignment in Items)
        {
            var result = results.FirstOrDefault(r => r.AssignmentNr == assignment.AssignmentNr);
            assignment.Result = result;
        }
    }


}
