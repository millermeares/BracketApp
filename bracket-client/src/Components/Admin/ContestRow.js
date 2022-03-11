import {Button} from 'react-bootstrap';
function ContestRow({name, id, finalized, handleEditCompetitors, handleDeleteTournament}) {
    
    return (
        <tr>
            <td>{name}</td>
            <td>
                <Button variant="secondary" onClick={() => handleEditCompetitors(id)}>
                    Edit Competitors
                </Button>
            </td>
            <td>
                <Button variant="danger" disabled={finalized} onClick={() => handleDeleteTournament(id)}>
                    Delete
                </Button>
            </td>
        </tr>
    )
}

export default ContestRow;