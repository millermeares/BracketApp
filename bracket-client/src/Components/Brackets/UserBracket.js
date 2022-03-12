import NCAABracket from "./NCAABracket";
import api from '../Services/api';
import {useAuth} from '../Entry/Auth';
function UserBracket({tournament, id, name, owner, completed, champTotalPoints, creationTime}) {
    let auth = useAuth();
    //todo: functions like onPickMade or something
    let onMakePick = async (gameID, winnerID) => {
        let pick = {
            BracketID: id, 
            GameID: gameID,
            CompetitorID: winnerID,
            TournamentID: tournament.id
        }
        let obj = {
            Token: auth.token,
            Pick: pick
        }
        return api.post("/savepick", obj).then(response => {
            if(!response.data.valid) {
                alert(response.data.payload);
                return false;
            }
            return true;
        }).catch(error => {
            console.log(error);
            return false;
        })
    }
    let bracket_props = {...tournament, beforeSetWinnerPromise:onMakePick }
    return (
        <NCAABracket {...bracket_props} />
    )
}

export default UserBracket;