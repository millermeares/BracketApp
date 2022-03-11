import {useAuth} from '../Entry/Auth'; 
import {useNavigate} from 'react-router-dom'
import { useEffect, useState } from 'react';
import {Container, Row, Col} from 'react-bootstrap';
import ContestInput from './ContestInput';
import ContestTable from './ContestTable';
import api from '../Services/api';
import EditContestCompetitors from './EditContestCompetitors';
import {Routes} from 'react-router-dom'
function ConfigTournament() {
    let auth = useAuth();
    let navigate = useNavigate();
    const [contests, setContests] = useState(null);
    const [editingTournamentID, setEditingTournamentID] = useState(null);
    

    useEffect(() => {
        if (!contests) {
            api.post("/alltournaments", auth.token).then(response => {
                if (!response.data.valid) {
                    alert(response.data.payload);
                    return;
                }
                setContests(response.data.payload);
            }).catch(err => {
                console.log(err);
            });
        }
    })

    let onCreateContest = (tournament) => {
        setContests([...contests, tournament])
    }

    let onTournamentDeleted = (tournamentID) => {
        let index_of_id = contests.map(c => c.id).indexOf(tournamentID);
        contests.splice(index_of_id, 1);
        setContests([...contests])
        
    };

    let onEditCompetitorsClick = (tournamentID) => {
        setEditingTournamentID(tournamentID);
    }
    let onSuccessfulFinalize = (id) => {
        let tournament = getTournament(id);
        tournament.finalized = true;
        setContests([...contests]);
    }
    let getTournament = (tournamentID, includeOnSuccessfulFinalize) => {
        if(includeOnSuccessfulFinalize) {
            return {...contests.find(t => t.id == tournamentID), onSuccessfulFinalize:onSuccessfulFinalize};
        } else {
            return {...contests.find(t => t.id ==tournamentID )}
        }
    }

    


    return (
        <Container className="tournament-config" fluid>
            <Row>
                <Col sm="4">
                    <ContestTable contests={contests} onTournamentDeleted={onTournamentDeleted} setEditingTournamentID={onEditCompetitorsClick} />
                </Col>
                <Col sm="4">
                    {!editingTournamentID ? null :
                        <EditContestCompetitors {...getTournament(editingTournamentID, true)} />
                    }
                </Col>
                <Col sm="4">
                    <ContestInput onCreateContest={onCreateContest} />
                </Col>
            </Row>
        </Container>
    )
}

export default ConfigTournament;