import axios from 'axios';
import {useEffect, useState} from 'react';
import NCAABracket from './NCAABracket'
function FakeTournament() {
    let [bracket, setBracket] = useState(null);
    useEffect(() => {
        if (!bracket) {
            axios.get("faketournamentskeleton").then(response => {
                console.log(response.data);
                setBracket(response.data);
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