import {Link} from 'react-router-dom';
import Navbar from 'react-bootstrap';
import Dashboard from './Dashboard';
import { useEffect } from 'react';
import axios from 'axios';
import GettingIn from '../Entry/GettingIn'
import Login from '../Entry/Login'
import {Navigate} from 'react-router-dom'
function Home({token, setToken, children}) {
    // a fully fleshed out application would pass components to that dashboard.
    // after the dashboard, it would render content.. right? how to render content hm
    // can i access state? or is state just exclusive to each individual component.
    
    // i'm not sure that this is teh best way to do this - pass props as the argument? 
    // maybe check if there's not a token and go to login if so?

    // useEffect(() => {
    //     axios("/weatherforecast").then(response => console.log(response.data)).catch(err => console.log(err));
    // })

    // if(!token) {
    //     return <GettingIn children={<Login setToken={setToken}/>} />
    // }
    const _userIsLoggedIn = function() {
        return (token) ? true : false;
      }
    
      if(!_userIsLoggedIn()) {
        return <Navigate to="/login" replace={true} />
      }
    return (
        <div className="home">
            <Dashboard /> 
            {children} 
        </div>
    )
}
export default Home;