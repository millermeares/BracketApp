import BootstrapTable from 'react-bootstrap-table-next';
import cellEditFactory from 'react-bootstrap-table2-editor';
import {useState} from 'react';
import {v4 as uuid} from 'uuid';
function EditableTable({columns, data, saveRowFunction, keyField}) {
    const [rowData, setRowData] = useState(data);
    let updateRow = (row) => {
        let index = rowData.indexOf(r => r[keyField] == row[keyField]);
        rowData[index] = row;
    }
    let handleSaveRow = (oldValue, newValue, row, column) => {
        console.log(row);
        saveRowFunction(row).then(success => {
            if(!success) {
                alert("error saving row");
                // set row data as current row data ( i think )
                setRowData([...rowData]);
                return;
            }
            // update row data to be accurate.
            updateRow(row);
        });
    }

    return (
        <BootstrapTable 
            keyField={keyField} 
            data={rowData} 
            columns={columns}
            cellEdit={cellEditFactory({
                keyField: keyField,
                mode: 'click', 
                blurToSave: true,
                afterSaveCell: handleSaveRow
                })}
        />
    )
}

export default EditableTable;