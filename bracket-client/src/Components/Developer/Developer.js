import {useAuth} from '../Entry/Auth';
import {useNavigate} from 'react-router-dom';
import {useEffect} from 'react';
import EditableTable from '../Generalized/EditableTable';

let makeProducts = () => {
    let products = [];
    for(let i = 0; i < 10; i++) {
        products.push({
            id: i, 
            name: 'product ' + i, 
            word: 'this is a word'
        });
    }
    return products;
}

const tableColumns = [
    {
        dataField: 'id', 
        text: 'product id'
    }, {
        dataField: 'name', 
        text: 'product name'
    }, {
        dataField: 'word', 
        text: 'product random word'
    }
]

function Developer() {
    let auth = useAuth();
    let navigate = useNavigate();
    useEffect(() => {
        if(!auth.hasPermission("developer")) {
            navigate("/");
            return;
        }
    });

    let onEditFunc = (row) => {
        console.log(row);
    }

    return (
        <div>
            <h1>Developer</h1>
            <EditableTable columns={tableColumns} data={makeProducts()} saveRowFunction={onEditFunc} keyField='id'/>
        </div>
    )
}
export default Developer;