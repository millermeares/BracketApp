import './App.css';
import Login from './Components/Entry/Login'
import 'bootstrap/dist/css/bootstrap.min.css';
import { BrowserRouter as Router, Link, Routes, Route } from 'react-router-dom'
import SignUp from './Components/Entry/SignUp'
import GettingIn from './Components/Entry/GettingIn'
import Home from './Components/Navigation/Home'
import LoggedOut from './Components/Entry/LoggedOut'
import {useState} from 'react';
function App() {
  // use state to check if logged in. if logged in, display dashboard. 
  // if not logged in, display login screen. 
  const [token, setToken] = useState();
  

  return (
    <div className="App">
      <Router>
        <Routes>
          <Route path="/" element={<Home token={token} />} />
          <Route path="/home"  element={<Home token ={token} />} />
          <Route path="/login" element={<GettingIn children={<Login setToken={setToken}/>} />} />
          <Route path="/signup" element={<GettingIn children={<SignUp setToken={setToken}/>} />} />
          <Route path="/logout" element={<LoggedOut />} />
        </Routes>
      </Router>
    </div>
  );
}

export default App;
