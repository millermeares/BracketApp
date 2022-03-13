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
import FakeTournament from './Components/Brackets/FakeTournament';
import Admin from './Components/Admin/Admin'
import Developer from './Components/Developer/Developer'
import FillOutBracket from './Components/Brackets/FillOutBracket';
import UserCompletedBracketsTable from './Components/Brackets/CompletedBrackets/UserCompletedBracketTable';
import CompletedBracket from './Components/Brackets/CompletedBrackets/CompletedBracket';
function App() {

  let default_element = <FillOutBracket />
  return (
    <div className="App">
      <AuthProvider>
        <Routes>
          <Route path="login" element={<GettingIn children={<Login />} />} />
          <Route path="signup" element={<GettingIn children={<SignUp />} />} />
          <Route path="completedbracket/:id" element={<CompletedBracket />} />
          <Route element={<RequireAuth />}>
            <Route element={<Layout />} >
              <Route index element={default_element} />
              <Route path="filloutbracket" element={<FillOutBracket />}/>
              <Route path="completedbrackets" element={<UserCompletedBracketsTable />} />
              <Route path="admin/*" element={<Admin />} />
              <Route path="developer" element={<Developer />} />
            </Route>

          </Route>
          <Route path="*" element={<NothingHere />}></Route>

        </Routes>
      </AuthProvider>
    </div>
  );
}

export default App;
