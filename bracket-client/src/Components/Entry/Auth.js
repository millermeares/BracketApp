import axios from 'axios';
import React from 'react';
import { useBootstrapPrefix } from 'react-bootstrap/esm/ThemeProvider';
import { Navigate, useNavigate, useLocation, Link } from 'react-router-dom';
let AuthContext = React.createContext(!null);

export function useAuth() {
    return React.useContext(AuthContext);
}

export function AuthStatus() {
    let auth = useAuth();
    let navigate = useNavigate();
    if(!auth.user) {

        return (
            <div>
                {/* <p>You are not logged in.</p>
                <Link to="/login">Login</Link>
                <br />
                <Link to="/signup">Sign Up</Link> */}
            </div>
            
            
        )
    }
    return (
        <p>
            Welcome {auth.user}!
            <button onClick={() => {
                auth.signout(() => navigate("/"));
            }}
            >
                Sign Out</button>
        </p>
    )
}

export function RequireAuth({children}) {
    let auth = useAuth();
    let location = useLocation();
    if(!auth.user) {
        return <Navigate to="/login" state={{from:location}} replace />
    }
    return children;
}

export function AuthProvider({ children }) {
    let [token, setToken] = React.useState(null);
    let signin = (logginInForm, callback) => {
        axios.post("/login", logginInForm)
            .then(response => {
                setToken(response.data.token);
                callback();
            }).catch(error => {
                console.log(error);
                console.log("error logging in: " + error)
            });
    }

    let signout = (callback) => {
        axios.post("/logout", token)
        .then(response => {
            setToken(null);
            callback();
        }).catch(error => {
            console.log(error);
            console.log("error logging out");
        });
    }

    let signup = (signUpForm, callback) => {
        axios.post("signup", signUpForm)
        .then(response => {
            setToken(response.data.token);
            callback();
        }).catch(error => {
            console.log(error);
            console.log("error signing up " + error);
        })
    }

    let value = {token, signin, signout, signup};
    return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}
