import {useAuth} from '../Entry/Auth';
import {useNavigate} from 'react-router-dom';
import {useEffect} from 'react';
function Developer() {
    let auth = useAuth();
    let navigate = useNavigate();
    useEffect(() => {
        if(!auth.hasPermission("developer")) {
            navigate("/");
            return;
        }
    });
    return (
        <div>Developer</div>
    )
}
export default Developer;