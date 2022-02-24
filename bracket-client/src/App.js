import './App.css';
import Login from './Components/Entry/Login'
import 'bootstrap/dist/css/bootstrap.min.css';
import { BrowserRouter as Router, Link, Routes, Route, useNavigate } from 'react-router-dom'
import SignUp from './Components/Entry/SignUp'
import GettingIn from './Components/Entry/GettingIn'
import Home from './Components/Navigation/Home'
import NothingHere from './Components/Navigation/NothingHere';
import useToken from './Components/Services/UseToken';

function App() {

  const {token, setToken} = useToken();

  let here = (token) ? true : false;
  console.log(here);
  console.log(token);
  if (!token) {
    console.log('are we returning this');
    return <GettingIn children={<Login setToken={setToken} />} />
  }
  if (!token) {
    console.log("sup");
  }

  return (
    <div className="App">
      <Routes>
        <Route path="/" element={<Home token={token} />} />
        <Route path="/home" element={<Home token={token} />} />
        <Route path="/signup" element={<GettingIn children={<SignUp />} />} />
        <Route path="/*" element={<NothingHere />} />
      </Routes>
    </div>
  );
}

export default App;
