import { useState } from 'react'
import { useAuth } from '../Entry/Auth';
import api from '../Services/api';
import {Row, Col, Form, Button} from 'react-bootstrap';
import {v4 as uuid} from 'uuid'
import '../../Styling/TournamentCSS.css'

let setDefaultFormSelectValues = (form) => {
    form.seed = 1;
    form.division = "South";
}

function AddNewCompetitor({ tournamentID, validateVsExistingCompetitors, onSuccessfulAdd }) {
    const [showAddCompetitorInput, setShowAddCompetitor] = useState();
    const [errors, setErrors] = useState({})
    const [form, setForm] = useState({});
    if(!form.seed) {
        setDefaultFormSelectValues(form);
    }
    let auth = useAuth();
    const setField = (field, value) => {
        console.log(value);
        setForm({
            ...form,
            [field]: value
        });
        if (!!errors[field]) setErrors({
            ...errors,
            [field]: null
        })
    }

    const findFormErrors = () => {
        const newErrors = validateVsExistingCompetitors(form);
        return newErrors;
    }

    const onSubmit = (e) => {
        e.preventDefault();
        const newErrors = findFormErrors(form);
        if(Object.keys(newErrors).length > 0) {
            alert(JSON.stringify(newErrors));
            return;
        }
        createNewCompetitor(form);
    }

    const createNewCompetitor = (form) => {
        let obj = {
            Token: auth.token,
            Competitor: form, 
            TournamentID: tournamentID
        }
        api.post("/createcompetitor", obj).then(response => {
            if(!response.data.valid) {
                alert(response.data.payload);
                return;
            } 
            onSuccessfulAdd(response.data.payload);
        }).catch(err => {
            console.log(err);
        })
    }

    let handleAddCompetitorClick = () => {
        setShowAddCompetitor(!showAddCompetitorInput);
    }

    let getButton = () => {
        if(showAddCompetitorInput) {
            return <Button variant="secondary" size="md" onClick={handleAddCompetitorClick}>Back</Button>
        } else {
            return <Button variant="primary" size="md" onClick={handleAddCompetitorClick}>Add Competitor</Button>
        }
    }

    let makeSeedSelect = () => {
        let seed_components = [];
        let seeds = getSeeds();
        for(let i = 0; i < seeds.length; i++) {
            seed_components.push(<option key={uuid()} value={seeds[i]}>{seeds[i]}</option>)
        }
        return (
            <Form.Select value={form.seed} size="sm" required onChange={(event) => setField('seed', event.target.value)}>
                {seed_components}
            </Form.Select>
        )
    }

    let makeDivisionSelect = () => {
        let division_components = [];
        let divisions = getDivisions();
        for (let i = 0; i < divisions.length; i++) {
            division_components.push(<option key={uuid()} value={divisions[i]}>{divisions[i]}</option>)
        }
        return (
            <Form.Select value={form.division} size="sm" required onChange={(event) => setField('division', event.target.value)}>
                {division_components}
            </Form.Select>
        )
    }

    let getDivisions = () => {
        return ["A", "B", "C", "D"]
    }

    let getSeeds = () => {
        let result = [];
        for (let i = 0; i < 16; i++) {
            result.push(i + 1);
        }
        return result;
    }

    return (
        <div>
            {getButton()}
            {
                showAddCompetitorInput ?
                    <Form onSubmit={onSubmit}>
                        <Row>
                            <Col>
                                <Form.Group className="mb-3" controlid="formCompetitorName">
                                    <Form.Label>Team Name</Form.Label>
                                    <Form.Control type="text" placeholder="New Competitor" required onChange={(event) => setField('name', event.target.value)}>
                                    </Form.Control>
                                    <Form.Control.Feedback type='invalid'>{errors.name}</Form.Control.Feedback>
                                </Form.Group>
                            </Col>
                            <Col>
                                <Form.Group className="mb-3" controlid="formSeed">
                                    <Form.Label>Seed</Form.Label>
                                    {makeSeedSelect()}
                                </Form.Group>
                            </Col><Col>
                                <Form.Group>
                                    <Form.Label>Division</Form.Label>
                                    {makeDivisionSelect()}
                                </Form.Group>
                            </Col>
                        </Row>
                        <Button variant="secondary" type="submit">Submit</Button>
                        
                    </Form>
                    : null
            }

        </div>

    )
}

export default AddNewCompetitor;

