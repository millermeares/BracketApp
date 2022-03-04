import axios from 'axios';
import {useEffect, useState} from 'react';
import Bracket from './Bracket'
function FakeTournament() {
    let [bracket, setBracket] = useState(null);
    useEffect(() => {
        if (!bracket) {
            axios.get("faketournament").then(response => {
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
            <Bracket {...bracket} />
        </div>
    );
}

export default FakeTournament;