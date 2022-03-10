import {Button} from 'react-bootstrap'
function CompetitorRow({name, id, seed, division, handleDeleteCompetitor}) {
    return (
        <tr>
            <td>{name}</td>
            <td>{division}</td>
            <td>{seed}</td>
            <td>
                <Button variant="danger" onClick={() => handleDeleteCompetitor(id)}>Delete</Button>
            </td>
        </tr>
    )    
}

export default CompetitorRow;