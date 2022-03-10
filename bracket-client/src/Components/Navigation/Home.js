import Dashboard from './Dashboard';
import {Outlet} from 'react-router-dom'
function Home() {
    return (
        <div className="home">
            <Outlet />
        </div>
    )
}
export default Home;