import {Link} from 'react-router-dom'
function LoggedOut() {
    return (
        <div>
            <p>You have been logged out.</p>
            <Link to="/login">Login</Link>
        </div>
    )
}
export default LoggedOut;