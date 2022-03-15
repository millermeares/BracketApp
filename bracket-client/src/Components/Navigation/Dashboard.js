import { Nav, Navbar } from 'react-bootstrap';
import { Link } from 'react-router-dom';
import { useNavigate } from 'react-router-dom';
import Container from 'react-bootstrap/Container'
import { useAuth } from '../Entry/Auth';
import { Outlet } from 'react-router-dom';
import {v4 as uuid} from 'uuid';

let getRoleDependentComponents = (auth) => {
    let roles = auth.getRoles();
    let components = []
    for(let i =0; i < roles.length; i++) {
        let role_link = "/" + roles[i].name;
        components.push(
        <Nav key={uuid()} className="me-auto">
            <Link to={role_link}>{roles[i].name}</Link>
        </Nav>)
    }
    return components;
}

function Dashboard() {
    
    let navigate = useNavigate();
    let auth = useAuth();
    let logout = (e) => {
        e.preventDefault();
        auth.signout(() => {
            navigate("../");
        });
    }
    let role_dependent_components = getRoleDependentComponents(auth);
    return (
        <div className="dashboard">
            <Navbar bg="light">
                <Container fluid>
                    <Nav className="me-auto">
                        <Link to="/filloutbracket">Fill Out Bracket</Link>
                    </Nav>
                    <Nav className="me-auto">
                        <Link to="/completedbrackets">Completed Brackets</Link>
                    </Nav>
                    <Nav className="me-auto">
                        <Link to="/userexposurereport">Exposure Report</Link>
                    </Nav>
                    {role_dependent_components}
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