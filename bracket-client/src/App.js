import './App.css';
import Login from './Components/Entry/Login'
import 'bootstrap/dist/css/bootstrap.min.css';
import { BrowserRouter as Router, Link, Routes, Route, useNavigate } from 'react-router-dom'
import SignUp from './Components/Entry/SignUp'
import GettingIn from './Components/Entry/GettingIn'
import Home from './Components/Navigation/Home'
import NothingHere from './Components/Navigation/NothingHere';
import Layout from './Components/Layout'
import { RequireAuth, AuthProvider } from './Components/Entry/Auth';
import MakeBracket from './Components/Brackets/MakeBracket';
import FakeTournament from './Components/Brackets/FakeTournament';
function App() {

let home_element = <Home />
  return (
    <div className="App">
      <AuthProvider>
        <Routes>
          <Route element={<Layout />}>
            <Route path="login" element={<GettingIn children={<Login />} />} />
            <Route path="signup" element={<GettingIn children={<SignUp />} />} />
            <Route element={<RequireAuth />}>
              <Route index element={home_element} />
              <Route path="home" element={home_element}>
                <Route path="makebracket" element={<MakeBracket />} />
                <Route path="faketournament" element={<FakeTournament />} />
              </Route>
            </Route>
            <Route path="*" element={<NothingHere />}></Route>

          </Route>
        </Routes>
      </AuthProvider>
    </div>
  );
}

export default App;
