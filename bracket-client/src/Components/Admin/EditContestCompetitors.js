import {useState, useEffect} from 'react';
import {useAuth} from '../Entry/Auth'
import api from '../Services/api'
import {Button, Table} from 'react-bootstrap';
import AddNewCompetitor from './AddNewCompetitor'
import CompetitorTable from './CompetitorTable';
function EditContestCompetitors({id, name, finalized, onSuccessfulFinalize}) {
    let auth = useAuth();
    const [competitors, setCompetitors] = useState(null);
    const [competitorTournamentID, setCompetitorTournamentID] = useState(id); // not necessarily an ideal solution - there's like 0.25 second where the info is wrong.

    let loadedContestsCompetitorHasChanged = () => {
        return ((competitors) && id != competitorTournamentID);
    }

    useEffect(() => {
        if(!competitors || loadedContestsCompetitorHasChanged()) {
            api.post("/getcompetitors", {id:id}).then(response => {
                if(!response.data.valid) {
                    alert(response.data.payload);
                    return;
                }
                setCompetitors(response.data.payload);
                setCompetitorTournamentID(id);
            }).catch(err => {
                console.log(err);
            });
        } 
    });

    let validationVsCurrentCompetitors = (newCompetitor) => {
        if(competitors.some(c => c.name.trim() == newCompetitor.name.trim())) {
            return {'name':'name already in use'};
        }
        if(competitors.some(c => c.seed== newCompetitor.seed && c.division == newCompetitor.division)) {
            return {'seed':'division/seed combination already in use for this tournament'}
        }
        return false;
    }

    let onSuccessfulCompetitorAdd = (competitor) => {
        competitors.push(competitor);
        setCompetitors([...competitors]);
    }
    let onSuccessfulCompetitorDelete= (competitor) => {
        let index_of_id = competitors.indexOf(c => c.id == competitor.id);
        competitors.splice(index_of_id, 1);
        setCompetitors([...competitors]);
    }

    let findCompetitor = (id) => {
        return competitors.find(c => c.id == id);
    }

    let handleDeleteCompetitor = (competitorID) => {
        let competitor = findCompetitor(competitorID);
        let obj = {
            Token: auth.token, 
            Competitor: competitor
        }
        api.post("/deletecompetitor", obj).then(response => {
            if(!response.data.valid) {
                alert(response.data.payload);
                return;
            }
            onSuccessfulCompetitorDelete(competitor)
        }).catch(error => {
            console.log(error);
        });
    }

    let handleFinalize = () => {
        if(competitors.length != 64) {
            alert("must have 64 competitors");
            return;
        }
        let obj = {
            TournamentID: id,
            Token: auth.token
        }
        api.post("/finalizetournament", obj).then(response => {
            if(!response.data.valid) {
                alert(response.data.payload);
                return;
            }
            onSuccessfulFinalize(id);
        }).catch(error => {
            console.log(error);            
        })
    }

    if(!competitors) {
        return <div>Loading Competitors...</div>
    }

    let disable_finalize = competitors.length != 64;


    return (
        <div>
            <h3>{finalized ? "Finalized" : "Editing"} {name}</h3>
            {finalized ? null : 
            <div>
                <Button variant="danger" disabled={disable_finalize} onClick={handleFinalize}>Finalize Competitors</Button>
                <AddNewCompetitor tournamentID={id} onSuccessfulAdd={onSuccessfulCompetitorAdd} validateVsExistingCompetitors={validationVsCurrentCompetitors} />
            </div>
            
            }
            <CompetitorTable competitors={competitors} handleDeleteCompetitor={handleDeleteCompetitor} allowDelete={!finalized}/>
        </div>
    )
}

export default EditContestCompetitors;