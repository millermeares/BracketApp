import { Nav, Navbar } from 'react-bootstrap';
import { useNavigate } from 'react-router-dom';
import Container from 'react-bootstrap/Container'
function Dashboard() {
    let navigate = useNavigate();
    let logout = (e) => {
        localStorage.clear();
        navigate("/home");
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