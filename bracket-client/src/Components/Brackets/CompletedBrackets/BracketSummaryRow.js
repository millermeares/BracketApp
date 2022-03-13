
import {Button} from 'react-bootstrap'
function BracketSummaryRow({bracketID, winnerName, tournamentName, creationDate, completionDate, onViewBracketClick}) { // could eventually have like a 'chalkiness' rating.
    console.log(completionDate);
    let get_date_string = (str) => {
        let date = new Date(Date.parse(str));
        return date.toLocaleString(date)
    }
    return (
        <tr>
            <td>{tournamentName}</td>
            <td>{winnerName}</td>
            <td>{get_date_string(completionDate)}</td>
            <td><Button onClick={() => onViewBracketClick(bracketID)}>View</Button></td>
        </tr>
    )
}
export default BracketSummaryRow