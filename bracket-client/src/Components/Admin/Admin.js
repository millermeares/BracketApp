import {useAuth} from '../Entry/Auth'; 
import {useNavigate} from 'react-router-dom'
import { useEffect, useState } from 'react';
import ContestInput from './ContestInput';
import ContestTable from './ContestTable';
import api from '../Services/api';
import EditContestCompetitors from './EditContestCompetitors';
import {Routes, Route, Link, Outlet} from 'react-router-dom'
import ConfigTournament from './ConfigTournament';
import SeedDataConfig from './SeedDataConfig';
import {Nav, Container, Row, Col} from 'react-bootstrap';
function Admin() {
    let auth = useAuth();
    let navigate = useNavigate();
    
    useEffect(() => {
        if (!auth.hasPermission("admin")) {
            navigate("/");
            return;
        }
    });




    return (
        <div className="admin">
            <h1>Admin</h1>

            <Container>
                <Row>
                    <Col sm="2">
                        <Nav defaultActiveKey="tournamentconfig" className="flex-column">
                            <Link to="tournamentconfig">Tournament Config</Link>
                            <Link to="seeddataconfig">SeedData Config</Link>
                        </Nav>
                    </Col>
                    <Col>
                    <div>this is a placerholder menu for now i don't want to style it hi ryan</div>
                        <Outlet />
                    </Col>
                </Row>
            </Container>
            <Routes>
                <Route index element={<ConfigTournament />} />
                <Route path="tournamentconfig" element={<ConfigTournament />} />
                <Route path="seeddataconfig" element={<SeedDataConfig />} />
            </Routes>
        </div>
        
    )
}

export default Admin;