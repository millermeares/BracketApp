import api from '../Services/api'
import React from 'react';
import { useBootstrapPrefix } from 'react-bootstrap/esm/ThemeProvider';
import { Navigate, useNavigate, useLocation, Link, Outlet } from 'react-router-dom';
let AuthContext = React.createContext(!null);
export function useAuth() {
    return React.useContext(AuthContext);
}

export function AuthStatus() { // i don't think this is ever used now.
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
    if(storage_token != null) {
        try {
            storage_token = JSON.parse(storage_token);
        }catch(err) {
            localStorage.clear();
            storage_token = null;
        }
        
    }
    let [token, setToken] = React.useState(storage_token);

    let signin = (logginInForm, callback, errorCallback) => {
        api.post("/login", logginInForm)
            .then(response => {
                if(!response.data.valid) {
                    errorCallback(response.data.payload);
                    return;
                }
                setToken(response.data.payload);
                localStorage.setItem("token", JSON.stringify(response.data.payload));
                callback();
            }).catch(error => {
                console.log(error);
                console.log("error logging in: " + error)
            });
    }

    let signout = (callback) => {
        api.post("/logout", token)
        .then(response => {
            callback(); // maybe here maybe after.
        }).catch(error => {
            console.log(JSON.stringify(token));
            console.log(error);
        });
        setToken(null);
        localStorage.clear();
    }

    let signup = (signUpForm, callback, errorCallback) => {
        api.post("signup", signUpForm)
        .then(response => {
            if(!response.data.valid) {
                errorCallback(response.data.payload);
                return;
            }
            setToken(response.data.payload);
            localStorage.setItem("token", JSON.stringify(response.data.payload));
            callback();
        }).catch(error => {
            console.log(error);
            console.log("error signing up " + error);
        })
    }

    // probably shouldn't be relying on this value. 
    let hasPermission = (permission) => {
        if(!token) return false;
        if(!token.roles) return false;
        return token.roles.map(r => r.name.toLowerCase()).includes(permission.toLowerCase());
    }

    let getRoles = () => {
        if(!token) return [];
        if(!token.roles) return [];
        return token.roles;
    }
    let value = {token, signin, signout, signup, hasPermission, getRoles};
    return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}
