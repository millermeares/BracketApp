import Col from 'react-bootstrap/Col';
import Button from 'react-bootstrap/Button'
import Container from 'react-bootstrap/Container'
import Row from 'react-bootstrap/Row'
import Form from 'react-bootstrap/Form'
import {useState} from 'react'
import {BrowserRouter as Router, Link} from 'react-router-dom';
function Login() {
    const [pw, setPw] = useState();
    const [username, setUsername] = useState();
    //todo: send request to server
    const _onSubmit = function() {
        let obj = {Password: pw, Username: username};
        alert(JSON.stringify(obj));
    }

    //todo: forgot-password.
    return (
        <div className="getting-in">
            <Form onSubmit={_onSubmit}>
            <h1 className="jordan">Hi Jordan</h1>
            <Form.Group controlId="formBasicUsername">
                <Form.Control placeholder="enter username" onChange={(event) => setUsername(event.target.value)} value={username}>
                </Form.Control>
            </Form.Group>
                <Form.Group controlId="formBasicPassword">
                    <Form.Control placeholder="enter password" type="password" onChange={(event) => setPw(event.target.value)} value={pw} ></Form.Control>
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