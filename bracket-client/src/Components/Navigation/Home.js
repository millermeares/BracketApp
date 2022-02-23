import {Link} from 'react-router-dom';
import Navbar from 'react-bootstrap';
import Dashboard from './Dashboard';
import { useEffect } from 'react';
import axios from 'axios';
import GettingIn from '../Entry/GettingIn'
import Login from '../Entry/Login'
import {useNavigate} from 'react-router-dom'
function Home({token, children}) {

    
    


   

      
    return (
        <div className="home">
            <Dashboard /> 
            {children} 
        </div>
    )
}
export default Home;