import {AuthStatus} from "./Entry/Auth"
import { Outlet } from "react-router-dom";
import Dashboard from "./Navigation/Dashboard";
function Layout() {
    return (
        <div>
            <Dashboard />
            <Outlet />
        </div>
    )
}
export default Layout;