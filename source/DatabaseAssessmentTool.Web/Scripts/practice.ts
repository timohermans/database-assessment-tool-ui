import { EditorView, basicSetup } from "codemirror";
import { sql, MSSQL, SQLConfig } from "@codemirror/lang-sql";

type TableColumnInfo = {
    tableName: string;
    columnName: string;
    dataType: string;
}

interface Schema {
    [tableName: string]: Completion[]
};

interface Completion {
    label: string;
}

const schemaInfoElement: HTMLInputElement = document.querySelector('#schemaInfo');
const tableColumnItems: TableColumnInfo[] = JSON.parse(schemaInfoElement.value);
const schema: Schema = tableColumnItems.reduce((result, item: TableColumnInfo) => {
    if (!(item.tableName in result)) result[item.tableName] = [];
    result[item.tableName].push({
        label: item.columnName,
    } as Completion)
    return result;
}, {});

const sqlConfig: SQLConfig = {
    dialect: MSSQL,
    schema
}

let view = new EditorView({
    extensions: [basicSetup, sql(sqlConfig)],
    parent: document.body
});
