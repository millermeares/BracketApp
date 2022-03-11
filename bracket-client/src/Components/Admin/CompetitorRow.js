import {Button} from 'react-bootstrap'
function CompetitorRow({name, id, seed, division, handleDeleteCompetitor, allowDelete}) {
    return (
        <tr>
            <td>{name}</td>
            <td>{division}</td>
            <td>{seed}</td>
            {allowDelete ? <td>
                <Button variant="danger" onClick={() => handleDeleteCompetitor(id)}>Delete</Button>
            </td> : null}
            
        </tr>
    )    
}

export default CompetitorRow;