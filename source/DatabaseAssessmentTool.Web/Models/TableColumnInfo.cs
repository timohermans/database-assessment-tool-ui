namespace DatabaseAssessmentTool.Web.Models;

public class TableColumnInfo
{
    public required string TableName { get; set; }
    public required string ColumnName { get; set; }
    public required string DataType { get; set; }
    public required string IsNullableText { get; set; }
    public bool IsNullable => IsNullableText == "YES" ? true : false;
}
