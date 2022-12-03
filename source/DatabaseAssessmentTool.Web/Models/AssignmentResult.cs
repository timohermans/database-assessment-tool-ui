using Dapper.FluentMap.Mapping;
using System.Xml;
using System.Xml.Serialization;

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

    public class QueryCheck
    {
        public required string CheckDescription { get; set; }
        public required double Points { get; set; }
        public required string CheckResults { get; set; }
    }

    [XmlRoot(ElementName = "Checks", Namespace = "")]
    public class QueryResult
    {
        public required string CheckDescription { get; set; }
        public required double Points { get; set; }
        public required string CheckResult { get; set; }
    }

    public class AssignmentResult
    {
        private string? _queryTextCheck;

        public required string Viewname { get; set; }
        public required string? AssignmentNr { get; set; }
        public required string? Delivered { get; set; }
        public required string? StructureOK { get; set; }
        public required string? ColumnsCompare { get; set; }
        public required string? DataCorrect { get; set; }
        public required int? MaxPoints { get; set; }
        public required string? QueryTextCheck
        {
            get => _queryTextCheck;
            set
            {
                _queryTextCheck = value;
                if (_queryTextCheck == null || !_queryTextCheck.StartsWith("<Checks")) return;
                using var stream = new StringReader(_queryTextCheck);
                var serializer = new XmlSerializer(typeof(QueryResult));
                QueryResult = (QueryResult?)serializer.Deserialize(stream);
            }
        }
        public required int? SizeMarginExceeded { get; set; }

        public QueryResult? QueryResult { get; private set; }
    }
}
