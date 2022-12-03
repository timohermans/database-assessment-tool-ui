import { EditorState } from "@codemirror/state"
import { EditorView, basicSetup } from "codemirror";
import { keymap } from "@codemirror/view"
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
const queryThatRan: HTMLInputElement = document.querySelector('#valRanQuery');
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

function executeQuery() {
    console.log("trying to execute");
    console.log(view.state.doc);
}

function executeQueryKeymap() {
    return keymap.of([{
        key: "Ctrl-Enter",
        run() { executeQuery(); return true }
    }])
}

const view = new EditorView({
    state: EditorState.create({
        doc: queryThatRan.value ?? "",
        extensions: [basicSetup, sql(sqlConfig), executeQueryKeymap()],
    }),
    parent: document.querySelector('#editor')
});

document.querySelector('#btnRunQuery').addEventListener('click', (event) => {
    const isFinalQueryElement: HTMLInputElement = document.querySelector('#valQuery');
    const queryElement: HTMLInputElement = document.querySelector('#valQuery');
    isFinalQueryElement.value = 'false';
    queryElement.value = view.state.doc.toString();
    (event.target as HTMLButtonElement).form.submit();
});
