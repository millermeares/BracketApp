import { Nav, Navbar } from 'react-bootstrap';
import { Link } from 'react-router-dom';
import { useNavigate } from 'react-router-dom';
import Container from 'react-bootstrap/Container'
import { useAuth } from '../Entry/Auth';
function Dashboard() {
    let navigate = useNavigate();
    let auth = useAuth();
    let logout = (e) => {
        e.preventDefault();
        auth.signout(() => {
            navigate("../");
        });
    }
    return (
        <div className="dashboard">
            <Navbar bg="light">
                <Container fluid>
                    <Nav className="me-auto">
                        <Link to="/home">Home</Link>
                    </Nav>
                    <Nav className="me-auto">
                        <Link to="/home/faketournament">Fake Tournament</Link>
                    </Nav>
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