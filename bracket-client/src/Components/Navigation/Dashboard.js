import { Nav, Navbar } from 'react-bootstrap';
import { Link } from 'react-router-dom';
import { useNavigate } from 'react-router-dom';
import Container from 'react-bootstrap/Container'
import { useAuth } from '../Entry/Auth';
import { Outlet } from 'react-router-dom';
import {v4 as uuid} from 'uuid';
import api from '../Services/api';
import {useState, useEffect} from 'react';
import { toHaveAccessibleDescription } from '@testing-library/jest-dom/dist/matchers';
let getRoleDependentComponents = (menuOptions) => {
    
    let components = []
    for(let i =0; i < menuOptions.length; i++) {
        components.push(
        <Nav key={uuid()} className="me-auto">
            <Link to={menuOptions[i].route}>{menuOptions[i].name}</Link>
        </Nav>)
    }
    return components;
}

function Dashboard() {
    const [menuOptions, setMenuOptions] = useState(null);

    let navigate = useNavigate();
    let auth = useAuth();
    let logout = (e) => {
        e.preventDefault();
        auth.signout(() => {
            navigate("../");
        });
    }

    useEffect(() => {
        if(!menuOptions) {
            api.post("/usermenuoptions", auth.token).then(response => {
                if(!response.data.valid) {
                    alert(response.data.payload);
                    return;
                }
                setMenuOptions(response.data.payload);
            }).catch(error => {
                console.log(error);
            });
        }
    })

    if(!menuOptions) {
        return <div>Loading...</div>
    }

    let dependent_components = getRoleDependentComponents(menuOptions);
    return (
        <div className="dashboard">
            <Navbar bg="light">
                <Container fluid>
                    {dependent_components}
                    <Nav>
                        <Navbar.Text className="justify-content-end">
                            <button onClick={logout}>Logout</button>
                        </Navbar.Text>
                    </Nav>
                </Container>
            </Navbar>
        </div>
    )
}
export default Dashboard;