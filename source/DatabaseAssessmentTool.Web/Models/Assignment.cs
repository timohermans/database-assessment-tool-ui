namespace DatabaseAssessmentTool.Web.Models;

public class Assignment
{
    public required string TypeAssignment { get; set; }
    public required string DatabaseName { get; set; }
    public required int AssignmentPrimaryNr { get; set; }
    public required string AssignmentNr { get; set; }
    public required string TimeLeft { get; set; }
    public required string AssignmentText { get; set; }
    public required double Points { get; set; }
    public required string Comments { get; set; }
    public required double TotalPoints { get; set; }
}
