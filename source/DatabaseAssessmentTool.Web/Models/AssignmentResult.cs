using Dapper.FluentMap.Mapping;

namespace DatabaseAssessmentTool.Web.Models
{
    public class AssignmentResultMap : EntityMap<AssignmentResult>
    {
        public AssignmentResultMap()
        {
            Map(result => result.QueryTextCheck)
                .ToColumn("Query Text Check");
        }
    }

    public class AssignmentResult
    {
        public required string Viewname { get; set; }
        public required string? AssignmentNr { get; set; }
        public required string? Delivered { get; set; }
        public required string? StructureOK { get; set; }
        public required string? ColumnsCompare { get; set; }
        public required string? DataCorrect { get; set; }
        public required int? MaxPoints { get; set; }
        public required string? QueryTextCheck { get; set; }
        public required int? SizeMarginExceeded { get; set; }
    }
}
