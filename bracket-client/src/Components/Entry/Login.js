import Col from 'react-bootstrap/Col';
import Button from 'react-bootstrap/Button'
import Container from 'react-bootstrap/Container'
import Row from 'react-bootstrap/Row'
import Form from 'react-bootstrap/Form'
import {useEffect, useState} from 'react'
import {BrowserRouter as Router, Link, useNavigate, useLocation} from 'react-router-dom';
import {useAuth } from './Auth';
import axios from 'axios';
function Login() {
    const [form, setForm] = useState({});
    let navigate = useNavigate();
    let auth = useAuth();
    let location = useLocation();
    let from = location.state?.from?.pathname || "/";
    console.log(from);
    const onSubmit = function(e) {
        e.preventDefault();
        
        auth.signin(form, () => {
            navigate(from, {replace: true});
        });

        
    }

    const setField = (field, value) => {
        setForm({
            ...form,
            [field]: value
        })
    }

    //todo: forgot-password.
    return (
        <div className="getting-in">

            <Form onSubmit={onSubmit}>
                <h1 className="jordan">bracket app</h1>
                <Form.Group controlId="formBasicUsername">
                    <Form.Control required placeholder="enter username" onChange={(event) => setField('username', event.target.value)} />
                </Form.Group>
                <Form.Group controlId="formBasicPassword">
                    <Form.Control required placeholder="enter password" type="password" onChange={(event) => setField('password', event.target.value)} />
                </Form.Group>
                <Button type="submit" variant="outline-primary">Submit</Button>
            </Form>
            <Link to="/signup">Don't have an account? Sign up here.</Link>
        </div>
    );
}
export default Login;