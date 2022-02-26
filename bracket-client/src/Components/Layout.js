import {AuthStatus} from "./Entry/Auth"
import { Outlet } from "react-router-dom";
function Layout({children}) {
    return (
        <div>
            <AuthStatus />
            {children}
            <Outlet />
        </div>
    )
}
export default Layout;