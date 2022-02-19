import {Container, Row, Col} from 'react-bootstrap'
function GettingIn(props) {
    return (
        <Container>
            <Row>
                <Col></Col>
                <Col>{props.children}</Col>
                <Col></Col>
            </Row>
        </Container>
    );
}
export default GettingIn;