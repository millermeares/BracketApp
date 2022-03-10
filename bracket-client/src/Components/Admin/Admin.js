import {useAuth} from '../Entry/Auth'; 
import {useNavigate} from 'react-router-dom'
import { useEffect, useState } from 'react';
import {Button} from 'react-bootstrap';
import ContestInput from './ContestInput';
import ProtectedApi from '../Services/ProtectedApi';
function Admin() {
    let auth = useAuth();
    let navigate = useNavigate();
    useEffect(() => {
        if(!auth.hasPermission("admin")) {
            navigate("/");
            return;
        }
    });

    



    return (
        <div>
            <h1>Admin</h1>
            <ContestInput />
        </div>
        
    )
}

export default Admin;