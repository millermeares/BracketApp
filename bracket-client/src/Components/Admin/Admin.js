import {useAuth} from '../Entry/Auth'; 
import {useNavigate} from 'react-router-dom'
import { useEffect, useState } from 'react';
import {Container, Row, Col} from 'react-bootstrap';
import ContestInput from './ContestInput';
import ContestTable from './ContestTable';
import api from '../Services/api';
import EditContestCompetitors from './EditContestCompetitors';
function Admin() {
    let auth = useAuth();
    let navigate = useNavigate();
    const [contests, setContests] = useState(null);
    const [editingTournamentID, setEditingTournamentID] = useState(null);
    useEffect(() => {
        if (!auth.hasPermission("admin")) {
            navigate("/");
            return;
        }
    });

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
        let copy_of_contests = contests;
        copy_of_contests.splice(index_of_id, 1);

        setContests([...copy_of_contests])
        
    };

    let onEditCompetitorsClick = (tournamentID) => {
        setEditingTournamentID(tournamentID);
    }

    let getTournament = (tournamentID) => {
        return contests.find(t => t.id == tournamentID);
    }


    return (
        <div className="tournament-config">
            <h1>Admin</h1>
            <Container>
                <Row>
                    <Col sm="4">
                        <ContestTable contests={contests} onTournamentDeleted={onTournamentDeleted} setEditingTournamentID={onEditCompetitorsClick} />
                    </Col>
                    <Col sm="4">
                        {!editingTournamentID ? null : 
                            <EditContestCompetitors {...getTournament(editingTournamentID)} />
                        }
                    </Col>
                    <Col sm="4">
                        <ContestInput onCreateContest={onCreateContest} />
                    </Col>
                </Row>
            </Container>
        </div>
        
    )
}

export default Admin;