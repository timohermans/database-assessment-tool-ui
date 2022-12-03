namespace DatabaseAssessmentTool.Web.Models;

public class RelationshipInfo
{
    public required string FkName { get; set; }
    public required string Table { get; set; }
    public required string Column { get; set; }
    public required string ReferencedTable { get; set; }
    public required string ReferencedColumn { get; set; }
}
