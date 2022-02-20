import Form from 'react-bootstrap/Form';
import Button from 'react-bootstrap/Button';
import { InputGroup } from 'react-bootstrap';
import {Link} from 'react-router-dom'
import {useState} from 'react';
import axios from 'axios';
function SignUp({ setToken }) {
    console.log("entering sign-in page");
    const [ form, setForm ] = useState({})
    const [ errors, setErrors ] = useState({})
    // todo: send request to server
    const _onSubmit = function(event) {
        const newErrors = findFormErrors();
        if(Object.keys(newErrors).length > 0) {
            setErrors(newErrors);
            event.preventDefault();
            return;
        } 
        // event.preventDefault();

        // now execute request.
        console.log(form);
        executeSignUpRequest(form);
    }

    const executeSignUpRequest = (data) => {
        // axios("/weatherforecast")
        // .then(response => {
        //     console.log(response.data);
        // }).catch(error => {
        //     console.log("Error fetching data: " + error);
        // });
        axios.post("/signup", data)
        .then(response => {
            console.log(response.data);
            setToken(response.data.Token);
        }).catch(error => {
            console.log("Error fetching data: " + error);
        })
    }

    const validateEmail = (email) => {
        return String(email)
          .toLowerCase()
          .match(
            /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
          );
      };

    const findFormErrors = () => {
        const {email, username, password, confirmpassword } = form;
        const newErrors = {};
        if(!validateEmail(email)) {
            newErrors.email = "invalid email address";
        }
        if(!username) {
            newErrors.username = "enter a username";
        }
        if(password.length < 8) {
            newErrors.password = "Password must be longer than 8 characters.";
        }
        if(confirmpassword !== password) {
            newErrors.confirmpassword = "passwords must match";
        }
        return newErrors;

    }

    const setField = (field, value) => {
        setForm({
          ...form,
          [field]: value
        })

        if(!!errors[field]) setErrors({
            ...errors,
            [field]:null
        })
      }


    
    return (
        <div>
            <h1>Sign Up</h1>
            <Form noValidate onSubmit={_onSubmit}>
                <Form.Group>
                    <Form.Label>Email</Form.Label>
                    <Form.Control type="email" placeholder="email" onChange={(event) => 
                        setField('email', event.target.value)} 
                        isInvalid = {!!errors.email}
                    />
                    <Form.Control.Feedback type='invalid'>{errors.email}</Form.Control.Feedback>
                </Form.Group>
                <Form.Group>
                    <Form.Label>Username</Form.Label>
                    <Form.Control required type="text" placeholder="username" 
                        onChange={(event) => setField('username', event.target.value)} 
                        isInvalid = {!!errors.username}
                    />
                    <Form.Control.Feedback type='invalid'>{errors.username}</Form.Control.Feedback>
                </Form.Group>
                <Form.Group>
                    <Form.Label>Password</Form.Label>
                    <Form.Control required type="password" placeholder="enter password" 
                        onChange={(event) => setField('password', event.target.value)} 
                        isInvalid = {!!errors.password}
                    />
                    <Form.Control.Feedback type='invalid'>{errors.password}</Form.Control.Feedback>

                </Form.Group>
                <Form.Group>
                    <Form.Label>Confirm Password</Form.Label>
                    <Form.Control required type="password" placeholder="confirm your password" 
                        onChange={(event) => setField('confirmpassword', event.target.value)}
                        isInvalid = {!!errors.confirmpassword}
                    />         
                    <Form.Control.Feedback type='invalid'>{errors.confirmpassword}</Form.Control.Feedback>
                </Form.Group>
                <Button type="submit" variant="outline-primary">Submit</Button>
            </Form>
            <Link to="/login">Already have an account? Login here.</Link>
        </div>
    );
}

export default SignUp;