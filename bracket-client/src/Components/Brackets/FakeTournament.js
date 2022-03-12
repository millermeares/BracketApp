import api from '../Services/api';
import {useEffect, useState} from 'react';
import NCAABracket from './NCAABracket'
function FakeTournament() {
    let [bracket, setBracket] = useState(null);
    useEffect(() => {
        if (!bracket) {
            api.get("/faketournament").then(response => {
                if(!response.data.valid) {
                    alert(response.data.payload);
                    return;
                }
                setBracket(response.data.payload);
            }).catch(err => {
                console.log(err);
            })
        }

    });


    if(!bracket) {
        return <div>Loading...</div>
    }
    let bracket_props = {...bracket, beforeSetWinnerPromise: async (gameId, winnerId) => {
        return true;
    }}
    return (
        <div>
            <NCAABracket {...bracket_props} />
        </div>
    );
}

export default FakeTournament;