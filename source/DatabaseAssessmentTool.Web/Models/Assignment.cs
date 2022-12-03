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
    public AssignmentResult? Result { get; set; }

    public bool IsTurnedIn => Result?.Delivered == "Y";
    public string Score
    {
        get
        {
            if (!IsTurnedIn) return $"~/{Points}";
            double pointsScored = 0;

            if (Result?.StructureOK == "OK" && Result?.ColumnsCompare == "OK" && Result?.DataCorrect == "OK")
            {
                pointsScored = Points - Math.Abs(Result?.QueryResult?.Points ?? 0);
            }

            return $"{pointsScored}/{Points}";
        }
    }
}
