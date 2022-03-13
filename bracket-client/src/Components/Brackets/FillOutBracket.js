import {useState, useEffect} from 'react';
import {useAuth} from '../Entry/Auth'
import api from '../Services/api'
import UserBracket from './UserBracket';
import {Button} from 'react-bootstrap';
import {v4 as uuid} from 'uuid';
function FillOutBracket() {
    const [fillingOutBracket, setFillingOutBracket] = useState(null);

    let auth = useAuth();
    let setBracketWithNewKey = (bracket) => {
        setFillingOutBracket({...bracket, key:uuid()})
    }
    useEffect(() => {
        if(!fillingOutBracket) {
            api.post("/neworlatestbracket", auth.token)
            .then(response => {
                if(!response.data.valid) {
                    alert(response.data.payload);
                    return;
                }
                setBracketWithNewKey(response.data.payload);
            }).catch(error => {
                console.log(error);
            });
        }
    });

    if(!fillingOutBracket) {
        return <div>Loading Bracket...</div>
    }

    
    

    let submitting_bracket = false;
    let submitBracket = () => {
        if(submitting_bracket) return;
        submitting_bracket = true;
        let obj = {
            ID: fillingOutBracket.id, 
            Token: auth.token
        }
        api.post("/finalizecontestentry", obj).then(response => {
            submitting_bracket = false;
            if(!response.data.valid) {
                alert(response.data.payload);
                return;
            }
            setBracketWithNewKey(response.data.payload);
            alert('submit successful');
        }).catch(err => {
            console.log(err);
            submitting_bracket = false;
        });
    }

    let allow_editing = true;
    let allowEditingFunc = () => {
        return allow_editing;
    }

    let autoFillBracket = () => {
        allow_editing = false;
        let obj = {
            ID: fillingOutBracket.id, 
            Token: auth.token
        }
        api.post("/smartfillbracket", obj).then(response => {
            if(!response.data.valid) {
                alert(response.data.payload);
                return;
            }
            setBracketWithNewKey({...response.data.payload, key:uuid()});
            allow_editing = true;
        }).catch(error => {
            console.log(error);
            allow_editing = true;
        })
    }

    let bracket_props = {...fillingOutBracket, key:fillingOutBracket.key, allowEditingFunc:allowEditingFunc}
    return (
        <div>
            <div className="d-flex justify-content-evenly">
                <Button onClick={submitBracket}>Submit Bracket</Button>
                <Button onClick={autoFillBracket}>SmartFill Bracket (still dumb for now)</Button>
            </div>
            
            <UserBracket {...bracket_props} />
        </div>
        
    )
}
export default FillOutBracket;