import axios from 'axios';
import {useEffect} from 'react';
function FakeTournament() {
    useEffect(() => {
        axios.get("faketournament").then(response => {
            console.log(response.data);
        }).catch(err => {
            console.log(err);
        })
    });
    return <div></div>
}

export default FakeTournament;