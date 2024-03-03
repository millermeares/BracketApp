import {Table} from 'react-bootstrap'
import CompetitorRow from './CompetitorRow';
import {v4 as uuid} from 'uuid'
import {api} from '../Services/api';
function CompetitorTable({competitors, allowDelete, handleDeleteCompetitor}) {
    
    function competitorComponents() {
        let components = []
        for(let i = 0; i < competitors.length; i++) {
            let props = {...competitors[i], key:uuid(), handleDeleteCompetitor:handleDeleteCompetitor, allowDelete: allowDelete }
            components.push(
                <CompetitorRow {...props} />
            )
        }
        return components;
    }

    return (
        <Table>
            <thead>
                <tr>
                    <th>Name</th>
                    <th>Division</th>
                    <th>Seed</th>
                    {allowDelete ? <th>Delete</th> : null}
                </tr>
            </thead>
            <tbody>
                {competitorComponents()}
            </tbody>
        </Table>
    )
}
export default CompetitorTable;