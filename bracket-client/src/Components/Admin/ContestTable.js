import {Table} from 'react-bootstrap';
import {useState, useEffect} from 'react';
import api from '../Services/api';
import {useAuth} from '../Entry/Auth';
import {v4 as uuid} from 'uuid';
import ContestRow from './ContestRow';
function ContestTable({contests, onTournamentDeleted, setEditingTournamentID}) {
    let auth = useAuth();

    
    
    function editCompetitorsClick(tournamentID) {
        setEditingTournamentID(tournamentID);
    }

    function deleteTournamentClick(tournamentID) {
        let obj = {
            Token: auth.token,
            TournamentID: tournamentID
        }
        api.post("/deletetournament", obj)
        .then(response => {
            if(!response.data.valid) {
                alert(response.data.payload);
                return;
            }
            onTournamentDeleted(tournamentID);
        }).catch(error => {
            console.log(error);
        });
    }

    function makeContestComponents(tournaments) {
        let components = [];
        for(let i = 0; i < tournaments.length; i++) {
            let props = {...tournaments[i], handleEditCompetitors: editCompetitorsClick, key: uuid(), handleDeleteTournament: deleteTournamentClick }
            components.push(
                <ContestRow {...props}/>
            )
        }
        return components;
    }
    if(!contests) {
        return <div>Loading...</div>
    }
    return (
        <Table>
            <thead>
                <tr>
                    <th>Name</th>
                    <th>Edit Competitors</th>
                    <th>Delete Tournament</th>
                </tr>
            </thead>
            <tbody>
                {makeContestComponents(contests)}
            </tbody>
        </Table>
    )
}
export default ContestTable;