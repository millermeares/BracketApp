import {v4 as uuid} from 'uuid';
import {Table} from 'react-bootstrap'
function ExposureReportTable(report_rounds) { // props is SortedList<Round, RoundReport>. i'm 
    console.log(report_rounds);
    let rounds = Object.keys(report_rounds);
    let amount_of_teams = report_rounds[rounds[0]].teamAppearances.length

    function getTeamAppearance(round, team_index) {
        return report_rounds[round].teamAppearances[team_index];
    }

    function makeRows() {
        let components = []
        for(let i = 0; i < amount_of_teams; i++) {
            
            components.push(makeSingleRow(i));
        }
        return components;
    }

    let makeSingleRow = (team_index) => {
        return (
            <tr key={team_index}>
                {row_column_components(team_index)}
            </tr>
        )
    }

    let team_appearance_string = (appearance) => {
        return appearance.competitorName + ": " + (appearance.percentString ? appearance.percentString : 0)  + "%";
    }

    let row_column_components = (team_index) => {
        let components = [];
        console.log(rounds);
        for(let i = 0; i < rounds.length; i++) {
            let appearance = getTeamAppearance(rounds[i], team_index);
            console.log(appearance);
            components.push(<td key={uuid()}>{team_appearance_string(appearance)}</td>)
        }
        return components;
    }
    function makeTitles() {
        return (<tr>
            {rounds.map(r => {
                return <th key={r}>{report_rounds[r].round.name}</th>
            })}
        </tr>)
    }

    console.log(makeRows())
    return (
        <div>
            <div>Exposure Report</div>
            <Table>
                <thead>
                    {makeTitles()}
                </thead>
                <tbody>
                    {makeRows()}
                </tbody>
            </Table>
            
            

        </div>
        
    )
}

export default ExposureReportTable;