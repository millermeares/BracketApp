import Col from 'react-bootstrap/Col';
import Button from 'react-bootstrap/Button'
import Container from 'react-bootstrap/Container'
import Row from 'react-bootstrap/Row'
import Form from 'react-bootstrap/Form'
import {useState} from 'react'
import {BrowserRouter as Router, Link} from 'react-router-dom';
import axios from 'axios';
function Login({setToken}) {
    const [form, setForm] = useState({});
    
    console.log("entering login");
    //todo: send request to server
    const onSubmit = function() {
        console.log(form);
        axios.post("/login", form)
            .then(response => {
                setToken(response.data.Token);
            }).catch(error => {
                console.log("error logging in: " + error)
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
            <br />
            <Link to="/">Shortcut link for development</Link>
        </div>
    );
}
export default Login;