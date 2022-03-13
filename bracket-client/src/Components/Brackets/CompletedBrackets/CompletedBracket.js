import {useEffect, useState} from 'react';
import {useParams} from 'react-router-dom'; 
import api from '../../Services/api';
import NCAABracket from '../NCAABracket';
function CompletedBracket() {

    const [bracket, setBracket] = useState(null);
    const [message, setMessage] = useState("Getting Bracket...");
    const { id } = useParams();
    console.log(id);
    useEffect(() => {
        if(!bracket) {
            api.get("/completedbracket/" + id).then(response => { // do i have to name the param? possibly. 
                if(!response.data.valid) {
                    setMessage(response.data.payload);
                    return;
                }
                setBracket(response.data.payload.tournament);
            }).catch(err => {
                console.log(err);
            });
        }
    });


    if(!bracket) {
        return <div>{message}</div>
    }

    let dontUpdateUIPromise = () => {
        return new Promise(resolve => {
            resolve(false);
        })
    }

    let bracket_props = {...bracket, beforeSetWinnerPromise: dontUpdateUIPromise}
    return (
        <NCAABracket {...bracket_props} />
    )


}

export default CompletedBracket;