import React from 'react';
import {Route, Navigate} from 'react-router-dom';

function PrivateRoute({component: Component, ...rest, token}) {
    return (
        <Route {...rest} render = {props => {
            if(!token) {
                return <Navigate to="/login" replace={true} />
            }
            return <Component {...props} />
        }} />

    )
}

export {PrivateRoute}