import { Nav, Navbar } from 'react-bootstrap';
import Container from 'react-bootstrap/Container'
function Dashboard() {
    return (
        <div className="dashboard">

            <Navbar bg="light">
                <Container fluid>
                    <Nav className="me-auto">
                        <Nav.Link href="/home">Home</Nav.Link>
                    </Nav>
                    <Nav>
                        <Navbar.Text className="justify-content-end">
                            <a href="/logout">Logout</a>
                        </Navbar.Text>
                    </Nav>
                </Container>
            </Navbar>
        </div>
    )
}
export default Dashboard;