import {useState, useEffect} from 'react';
import {Table, Button} from 'react-bootstrap'
import {useAuth} from '../../Entry/Auth';
import api from '../../Services/api'
import {v4 as uuid} from 'uuid';
import BracketSummaryRow from './BracketSummaryRow'
import {useNavigate} from 'react-router-dom';
function UserCompletedBracketsTable() {
    const [brackets, setBrackets] = useState(null);
    let auth = useAuth();
    let navigate = useNavigate();
    
    useEffect(() => {
        if(!brackets) {
            api.post("/bracketsforuser", auth.token).then(response => {
                if(!response.data.valid) {
                    alert(response.data.payload);
                    return;
                }
                setBrackets(response.data.payload);
            }).catch(error => {
                console.log(error);
            })
        }
    });

    if (!brackets) {
        return <div>Loading brackets...</div>
    }
    let onViewBracketClick = (bracketID) => {
        navigate("../completedbracket/" + bracketID);
    }

    function makeBracketComponents() {
        let components = [];
        brackets.sort((a, b) => {
            return b.bracketMax - a.bracketMax;
        });
        for (let i = 0; i < brackets.length; i++) {
            let props = { ...brackets[i], onViewBracketClick: onViewBracketClick, key:uuid() }
            components.push(
                <BracketSummaryRow {...props} />
            )
        }
        return components;
    }

    return (
        <Table>
            <thead>
                <tr>
                    <th>Tournament</th>
                    <th>Winner</th>
                    <th>Points Earned</th>
                    <th>Max Points</th>
                    <th>Completion Date</th>
                    <th>View</th>
                </tr>
            </thead>
            <tbody>
                {makeBracketComponents()}
            </tbody>
        </Table>
    )

}
export default UserCompletedBracketsTable;