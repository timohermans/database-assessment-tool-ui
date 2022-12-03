namespace DatabaseAssessmentTool.Web.Models;

public class AssignmentCollection
{
    public List<Assignment> Items { get; private set; }

    public AssignmentCollection(IEnumerable<Assignment> assignments)
    {
        Items = assignments.ToList();
    }

    public AssignmentCollection(IEnumerable<Assignment> assignments, IEnumerable<AssignmentResult> results) : this(assignments)
    {
        UpdateAssignmentsWithResults(results);
    }

    public void UpdateAssignmentsWithResults(IEnumerable<AssignmentResult> results)
    {
        foreach (var assignment in Items)
        {
            var result = results.FirstOrDefault(r => r.AssignmentNr == assignment.AssignmentNr);
            assignment.Result = result;
        }
    }

    public Assignment? GetBy(int id)
    {
        return Items.FirstOrDefault(a => a.AssignmentPrimaryNr == id);
    }


}
