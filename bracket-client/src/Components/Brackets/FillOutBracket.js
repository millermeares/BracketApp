import {useState, useEffect} from 'react';
import {useAuth} from '../Entry/Auth'
import api from '../Services/api'
import UserBracket from './UserBracket';
function FillOutBracket() {
    const [fillingOutBracket, setFillingOutBracket] = useState(null);

    let auth = useAuth();

    useEffect(() => {
        if(!fillingOutBracket) {
            
            api.post("/neworlatestbracket", auth.token)
            .then(response => {
                if(!response.data.valid) {
                    alert(response.data.payload);
                    return;
                }
                console.log(response.data.payload);
                setFillingOutBracket(response.data.payload);
            }).catch(error => {
                console.log(error);
            });
        }
    });

    if(!fillingOutBracket) {
        return <div>Loading Bracket...</div>
    }

    return (
        <UserBracket {...fillingOutBracket}/>
    )
}
export default FillOutBracket;