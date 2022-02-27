import { Nav, Navbar } from 'react-bootstrap';
import { useNavigate } from 'react-router-dom';
import Container from 'react-bootstrap/Container'
import { useAuth } from '../Entry/Auth';
function Dashboard() {
    let navigate = useNavigate();
    let auth = useAuth();
    let logout = (e) => {
        e.preventDefault();
        auth.signout(() => {
            navigate("...");
        });
    }
    return (
        <div className="dashboard">
            <Navbar bg="light">
                <Container fluid>
                    <Nav className="me-auto">
                        <Nav.Link href="/home">Home</Nav.Link>
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