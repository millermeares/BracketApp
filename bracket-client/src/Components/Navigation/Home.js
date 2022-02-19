import {Link} from 'react-router-dom';
import Navbar from 'react-bootstrap';
import Dashboard from './Dashboard';
import { useEffect } from 'react';
import axios from 'axios';
function Home(props) {
    // a fully fleshed out application would pass components to that dashboard.
    // after the dashboard, it would render content.. right? how to render content hm
    // can i access state? or is state just exclusive to each individual component.
    
    // i'm not sure that this is teh best way to do this - pass props as the argument? 
    useEffect(() => {
        axios("/weatherforecast")
        .then(response => {
            console.log(response.data);
        }).catch(error => {
            console.log("Error fetching data: " + error);
        })
    });
    return (
        <div className="home">
            <Dashboard /> 
            {props.children} 
        </div>
    )
}
export default Home;