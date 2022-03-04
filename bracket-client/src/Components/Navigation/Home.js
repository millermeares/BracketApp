import Dashboard from './Dashboard';
import {Outlet} from 'react-router-dom'
function Home() {
    return (
        <div className="home">
            <Dashboard /> 
            <Outlet />
        </div>
    )
}
export default Home;