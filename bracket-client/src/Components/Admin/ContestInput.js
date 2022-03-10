import {Form, Button} from 'react-bootstrap';
import {useState} from 'react';
import {useAuth} from '../Entry/Auth';
import api from '../Services/api';
function ContestInput() {
    const [showContestInput, setShowContestInput] = useState(false);
    const [form, setForm] = useState({});
    let auth = useAuth();
    const setField = (field, value) => {
        setForm({
          ...form,
          [field]: value
        })
      }

    const onSubmit = (e) => {
        e.preventDefault();
        let contest_name = form.contestname;
        let obj = {
            TournamentName: contest_name,
            Token: auth.token
        }
        console.log(obj);
        api.post("/createtournament", obj)
        .then(response => {
            if(!response.data.valid) {
                alert(response.data.payload);
                return;
            }
            setShowContestInput(false);
        }).catch(error => {
            console.log(error);
        });
        console.log(form);
    }
    let handleContestClick = () => {
        setShowContestInput(!showContestInput);
    }

    return (
        <div>
            <Button variant="primary" size="md" onClick={handleContestClick}>Create Contest</Button>
            {
                showContestInput ? 
                <Form onSubmit={onSubmit}>
                <Form.Group className="mb-3" controlid="formContest">
                    <Form.Control type="text" placeholder="Create Tournament" required onChange={(event) => setField('contestname', event.target.value)}>

                    </Form.Control>
                </Form.Group>
                <Button variant="secondary" type="submit">Submit</Button>
            </Form>

            : null
            }
            
        </div>

    )
}
export default ContestInput;