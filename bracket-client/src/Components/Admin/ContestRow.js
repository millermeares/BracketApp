import {Button} from 'react-bootstrap';
function ContestRow({name, id, handleEditCompetitors, handleDeleteTournament}) {
    
    return (
        <tr>
            <td>{name}</td>
            <td>
                <Button variant="secondary" onClick={() => handleEditCompetitors(id)}>
                    Edit Competitors
                </Button>
            </td>
            <td>
                <Button variant="danger" onClick={() => handleDeleteTournament(id)}>
                    Delete
                </Button>
            </td>
        </tr>
    )
}

export default ContestRow;