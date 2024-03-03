
import {Button} from 'react-bootstrap'
function BracketSummaryRow({bracketID, winnerName, tournamentName, creationDate, bracketMax, pointsEarned, completionDate, onViewBracketClick}) { // could eventually have like a 'chalkiness' rating.
    // This originally included the pointsEarned and potential max. that has to be optimized on server side so for now, we hide.
    let get_date_string = (str) => {
        let date = new Date(Date.parse(str));
        return date.toLocaleString(date)
    }
    return (
        <tr>
            <td>{tournamentName}</td>
            <td>{winnerName}</td>
            <td>{pointsEarned}</td>
            <td>{bracketMax}</td>
            <td>{get_date_string(completionDate)}</td>
            <td><Button onClick={() => onViewBracketClick(bracketID)}>View</Button></td>
        </tr>
    )
}
export default BracketSummaryRow