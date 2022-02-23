import Col from 'react-bootstrap/Col';
import Button from 'react-bootstrap/Button'
import Container from 'react-bootstrap/Container'
import Row from 'react-bootstrap/Row'
import Form from 'react-bootstrap/Form'
import {useEffect, useState} from 'react'
import {BrowserRouter as Router, Link, useNavigate} from 'react-router-dom';
import axios from 'axios';
function Login({token, setToken}) {
    const [form, setForm] = useState({});
    let navigate = useNavigate();

    const onSubmit = function() {
        axios.post("/login", form)
            .then(response => {
                setToken(response.data.token);
            }).catch(error => {
                console.log(error);
                console.log("error logging in: " + error)
            });

        
    }

    const setField = (field, value) => {
        setForm({
            ...form,
            [field]: value
        })
    }

    const userIsLoggedIn = () => {
        console.log(token);
        return (token) ? true : false;
    }

    
    useEffect(() => {
        if (userIsLoggedIn()) {
            console.log("user is logged in in login");
            console.log(token);
            navigate("../home/", { replace: false })
            
        }
    })

    const handleClick = () => {
        navigate("../home/", {replace:false});
    }

    //todo: forgot-password.
    return (
        <div className="getting-in">
            <button onClick={handleClick}>Go home</button>

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
            <br />
            <Link to="/">Shortcut link for development</Link>
        </div>
    );
}
export default Login;