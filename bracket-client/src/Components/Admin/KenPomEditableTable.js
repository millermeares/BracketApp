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
        dataField: 'competitorName', 
        text: 'Competitor Name'
    }, {
        dataField: 'offensiveEfficiency', 
        text: 'Offensive Efficiency', 
        validator: odds_validator
    }, {
        dataField: 'defensiveEfficiency', 
        text: 'Defensive Efficiency', 
        validator: odds_validator
    }, {
        dataField: 'overallEfficiency', 
        text: 'Overall Efficiency',         
        validator: odds_validator
    }, {
        dataField: 'tempo', 
        text: 'Tempo', 
        validator: odds_validator
    }
]

function KenPomEditableTable() {
    const [kenPomDatas, setKenPomDatas] = useState(null);
    let auth = useAuth();
    useEffect(() => {
        if(!kenPomDatas) {
            api.get("/kenpomdata").then(response => {
                if(!response.data.valid) {
                    alert(response.data.payload);
                    return;
                }
                setKenPomDatas(response.data.payload);
            }).catch(err => {
                console.log(err);
            });
        }
    });

    let saveKenPomDataFn = (row) => {
        let obj = {
            Token: auth.token,
            KenPom: row
        }
        return api.post("/savekenpom", obj);
    }
    if(!kenPomDatas) {
        return <div>Loading...</div>
    }

    let makeRows = (data) => {
        return data.map(d => ({
            key: d.competitor.id,
            competitorID: d.competitor.id,
            tournamentID: d.competitor.tournamentID,
            competitorName:d.competitor.name,
            offensiveEfficiency: d.data.offensiveEfficiency,
            defensiveEfficiency: d.data.defensiveEfficiency,
            overallEfficiency: d.data.overallEfficiency,
            tempo:d.data.tempo
        }));
    }
    console.log(kenPomDatas);
    console.log(makeRows(kenPomDatas));
    return (
        <EditableTable columns={tableColumns} data={makeRows(kenPomDatas)} saveRowFunction={saveKenPomDataFn} keyField='competitorName'/>
    )
}
export default KenPomEditableTable;