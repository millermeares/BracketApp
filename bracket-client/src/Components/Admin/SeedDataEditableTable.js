import api from '../Services/api';
import {useState, useEffect} from 'react';
import {useAuth} from '../Entry/Auth';
import EditableTable from '../Generalized/EditableTable';
let odds_validator = (newValue, row, column) => {
    if(isNaN(newValue)) {
        return {
            valid: false,
            message: "odds must be numeric"
        }
    }
    if(newValue < 0 || newValue > 100) {
        return {
            valid: false,
            message: "odds must be between 1 and 100"
        }
    }
    return true;
}

const tableColumns = [
    {
        dataField: 'seedID', 
        text: 'Seed'
    }, {
        dataField: 'finalFourOdds', 
        text: 'Final Four Odds', 
        validator: odds_validator
    }, {
        dataField: 'eliteEightOdds', 
        text: 'Elite Eight Odds', 
        validator: odds_validator
    }
]

function SeedDataEditableTable() {
    const [seedDatas, setSeedDatas] = useState(null);
    let auth = useAuth();
    useEffect(() => {
        if(!seedDatas) {
            api.get("/getseeddata").then(response => {
                if(!response.data.valid) {
                    alert(response.data.payload);
                    return;
                }
                setSeedDatas(response.data.payload);
            }).catch(err => {
                console.log(err);
            });
        }
    });

    let saveSeedDataFn = (row) => {
        console.log(row);
        let obj = {
            Seed: row,
            Token: auth.token
        }
        return api.post("/saveseeddata", obj); // this might not be right. will this data be inaccurate? does it matter? 
        // if not, why do i have it. could just have a showTable bool
        // i have two state representatins of row data - that's a little odd.
    }
    if(!seedDatas) {
        return <div>Loading...</div>
    }

    return (
        <EditableTable columns={tableColumns} data={[...seedDatas]} saveRowFunction={saveSeedDataFn} keyField='seedID'/>
    )
}
export default SeedDataEditableTable;