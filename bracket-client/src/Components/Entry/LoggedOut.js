import { useEffect } from 'react';
import {Link, useNavigate} from 'react-router-dom'
function LoggedOut({setToken}) {
    let navigate = useNavigate();
    useEffect(() => {
        setToken(null)
        navigate("../home")
    })
    return (
        <div>
            <p>You have been logged out.</p>
        </div>
    )
}
export default LoggedOut;