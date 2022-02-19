import Form from 'react-bootstrap/Form';
import Button from 'react-bootstrap/Button';
import {Link} from 'react-router-dom'
import {useState} from 'react';
function SignUp() {
    const [email, setEmail] = useState();
    const [fname, setFname] = useState();
    const [lname, setLname] = useState();
    const [username, setUsername] = useState();
    const [password, setPassword] = useState();
    const [confirmPassword, setConfirmPassword] = useState();

    // todo: send request to server
    const _onSubmit = function() {
        let obj = {
            Email: email,
            FirstName: fname,
            LastName: lname,
            Password: password, 
            ConfirmPassword: confirmPassword,
            Username: username
        }
        alert(obj);
    }

    return (
        <div>
            <h1>Sign Up</h1>
            <Form onSubmit={_onSubmit}>
                <Form.Group>
                    <Form.Label>Email</Form.Label>
                    <Form.Control type="email" placeholder="email" onChange={(event) => setEmail(event.target.value)} value={email}></Form.Control>
                </Form.Group>
                <Form.Group>
                    <Form.Label>Username</Form.Label>
                    <Form.Control type="text" placeholder="username" onChange={(event) => setUsername(event.target.value)} value={username}></Form.Control>
                </Form.Group>
                <Form.Group>
                    <Form.Label>First Name</Form.Label>
                    <Form.Control type ="text" placeholder="first name" onChange={(event) => setFname(event.target.value)} value={fname}></Form.Control>
                </Form.Group>
                <Form.Group>
                    <Form.Label>Last Name</Form.Label>
                    <Form.Control type ="text" placeholder="last name" onChange={(event) => setLname(event.target.value)} value={lname}></Form.Control>
                </Form.Group>
                <Form.Group>
                    <Form.Label>Password</Form.Label>
                    <Form.Control type="password" placeholder="enter password" onChange={(event) => setPassword(event.target.value)} value={password}></Form.Control>
                </Form.Group>
                <Form.Group>
                    <Form.Label>Confirm Password</Form.Label>
                    <Form.Control type="password" placeholder="confirm your password" onChange={(event) => setConfirmPassword(event.target.value)} value={confirmPassword}></Form.Control>
                </Form.Group>
                <Button type="submit" variant="outline-primary">Submit</Button>
            </Form>
            <Link to="/login">Already have an account? Login here.</Link>
        </div>
    );
}

export default SignUp;