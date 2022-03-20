import '../../Styling/BracketCSS.css'
import Team from './Team'
function TeamWithPredictions({predictedTeam, actualTeam, renderSeed, handleTeamClicked, isLeft}) {
    // todo: do this for real
    let props = {...predictedTeam, renderSeed: renderSeed, handleTeamClicked: handleTeamClicked ,isLeft};
    return <Team {...props} />
}
export default TeamWithPredictions;