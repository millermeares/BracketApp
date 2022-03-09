import api from '../Services/api'
import React from 'react';
import { useBootstrapPrefix } from 'react-bootstrap/esm/ThemeProvider';
import { Navigate, useNavigate, useLocation, Link, Outlet } from 'react-router-dom';
let AuthContext = React.createContext(!null);
export function useAuth() {
    return React.useContext(AuthContext);
}

export function AuthStatus() {
    let auth = useAuth();
    
    return (
        <div></div>
    )
}

export function RequireAuth() {
    let auth = useAuth();
    let location = useLocation();
    if(!auth.token) {
        return <Navigate to="/login" state={{from:location}} replace />
    }
    return <Outlet />
}

export function AuthProvider({ children }) {
    let storage_token = localStorage.getItem("token");
    let [token, setToken] = React.useState(storage_token);

    let signin = (logginInForm, callback, errorCallback) => {
        api.post("/login", logginInForm)
            .then(response => {
                if(!response.data.valid) {
                    errorCallback(response.data.payload);
                    return;
                }
                setToken(response.data.payload);
                localStorage.setItem("token", response.data.payload);
                callback();
            }).catch(error => {
                console.log(error);
                console.log("error logging in: " + error)
            });
    }

    let signout = (callback) => {
        setToken(null);
        localStorage.clear();
        api.post("/logout", token)
        .then(response => {
            callback(); // maybe here maybe after.
        }).catch(error => {
            console.log(error);
            console.log("error logging out");
        });
    }

    let signup = (signUpForm, callback, errorCallback) => {
        api.post("signup", signUpForm)
        .then(response => {
            if(!response.data.valid) {
                errorCallback(response.data.payload);
                return;
            }
            setToken(response.data.payload);
            localStorage.setItem("token", response.data.payload);
            callback();
        }).catch(error => {
            console.log(error);
            console.log("error signing up " + error);
        })
    }

    let value = {token, signin, signout, signup};
    return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}
