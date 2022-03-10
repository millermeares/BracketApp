import {useState, useEffect} from 'react';
import {useAuth} from '../Entry/Auth'
import api from '../Services/api'
import {Button, Table} from 'react-bootstrap';
function EditContestCompetitors({id, name}) {

    const [competitors, setCompetitors] = useState(null);

    useEffect(() => {
        if(!competitors) {
            api.post("/getcompetitors", {id:id}).then(response => {
                if(!response.data.valid) {
                    alert(response.data.payload);
                    return;
                }
                setCompetitors(response.data.payload);
            }).catch(err => {
                console.log(err);
            });
        }
    });


    let onAddCompetitorClick = () => {

    }

    function makeCompetitorComponents() {
        let components = [];
        for(let i =0 ; i < competitors.length; i++) {
            
        }
        return components;
    }




    if(!competitors) {
        return <div>Loading Competitors...</div>
    }

    return (
        <div>
            <h2>Editing {name}</h2>
            <Button variant="secondary" onClick={onAddCompetitorClick}>Add Competitor</Button>
            {
                makeCompetitorComponents()
            }
        </div>
    )
}

export default EditContestCompetitors;