import './App.css';
import Login from './Components/Entry/Login'
import 'bootstrap/dist/css/bootstrap.min.css';
import { BrowserRouter as Router, Link, Routes, Route, useNavigate } from 'react-router-dom'
import SignUp from './Components/Entry/SignUp'
import GettingIn from './Components/Entry/GettingIn'
import Home from './Components/Navigation/Home'
import LoggedOut from './Components/Entry/LoggedOut'
import NothingHere from './Components/Navigation/NothingHere';
import {useState} from 'react';
function App() {
  const [token, setToken] = useState(null);
  if(!token) {
    return <GettingIn children={<Login setToken={setToken} />} />
  }


  return (
    <div className="App">
      <Routes>
        <Route path="/" element={<Home token={token} />} />
        <Route path="/home" element={<Home token={token} />} />
        <Route path="/signup" element={<GettingIn children={<SignUp />} />} />
        <Route path="/logout" element={<LoggedOut setToken={setToken}/>} />
        <Route path="/*" element={<NothingHere />} />
      </Routes>
    </div>
  );
}

export default App;
