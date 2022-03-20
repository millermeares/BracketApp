import api from '../../Services/api';
import {useEffect, useState} from 'react';
import NCAABracket from '../../Brackets/NCAABracket'
import {useAuth} from '../../Entry/Auth';
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

    // okay so now we're going to set predictedCompetitors to competitors where we can. 
    let bracket_props = {...tournament, beforeSetWinnerPromise: beforeSetWinnerPromise}
    return (
        <NCAABracket {...bracket_props} />
    )
}

export default EnterTournamentOutcome;