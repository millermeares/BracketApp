import api from '../Services/api';
import {useEffect, useState} from 'react';
import NCAABracket from './NCAABracket'
function FakeTournament() {
    let [bracket, setBracket] = useState(null);
    useEffect(() => {
        if (!bracket) {
            api.get("/faketournamentskeleton").then(response => {
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
    return (
        <div>
            <NCAABracket {...bracket} />
        </div>
    );
}

export default FakeTournament;