import api from '../../Services/api';
import {useEffect, useState} from 'react';
import NCAABracket from '../../Brackets/NCAABracket'
import {useAuth} from '../../Entry/Auth';
import {Button} from 'react-bootstrap'

let modifyTournamentObjectForAdmin = (tournament) => {
    setPredictionsEqualToReality(tournament.championshipGame);
}

let setPredictionsEqualToReality = (game) => {
    if(!game) return;
    if(!game.predictedCompetitor1) {
        game.predictedCompetitor1 = game.competitor1;
    }
    if(!game.predictedCompetitor2) {
        game.predictedCompetitor2 = game.competitor2;
    }
    if(!game.predictedWinner) {
        game.predictedWinner = game.winner;
    }
    setPredictionsEqualToReality(game.rightGame)
    setPredictionsEqualToReality(game.leftGame);
}

function EnterTournamentOutcome({tournamentid}) {
    const [tournament, setTournament] = useState(null);
    let auth = useAuth();
    useEffect(() => {
        if (!tournament) {
            api.get("/activetournament").then(response => {
                if(!response.data.valid) {
                    alert(response.data.payload);
                    return;
                }
                let temp_tournament = response.data.payload;
                modifyTournamentObjectForAdmin(temp_tournament);
                setTournament(temp_tournament);
            }).catch(error => {
                console.log(error);
            })
        }
    });
    
    if(!tournament) {
        return <div>Loading Active Tournament...</div>
    }

    let beforeSetWinnerPromise = async (gameID, winnerID) => {
        let outcome = {
            GameID: gameID,
            CompetitorID: winnerID,
            TournamentID: tournament.id
        }
        let obj = {
            Token: auth.token,
            Outcome: outcome
        }
        console.log(obj);
        return api.post("/savegameoutcome", obj).then(response => {
            if(!response.data.valid) {
                alert(response.data.payload);
                return false;
            }
            return true;
        }).catch(error => {
            console.log(error);
            return false;
        });
    }

    let triggerBracketResultsUpdate = () => {
        let success = window.confirm("Are you sure you want to trigger an update of bracket results? This is a long process and should be done infrequently.")
        if (!success) {
            return;
        } 
        api.post("/asyncupdatebracketresults", auth.token).then(response => {
            if(!response.data.valid) {
                alert(response.data.payload);
                return;
            }
            alert("Asynchronous update of bracket results triggered.");
        }).catch(error => {
            console.log(error);
        })
    }

    // okay so now we're going to set predictedCompetitors to competitors where we can. 
    let bracket_props = {...tournament, beforeSetWinnerPromise: beforeSetWinnerPromise}
    return (
        <div>
            <Button onClick={triggerBracketResultsUpdate}>Trigger Bracket Results Update</Button>
            <NCAABracket {...bracket_props} />
        </div>
        
    )
}

export default EnterTournamentOutcome;