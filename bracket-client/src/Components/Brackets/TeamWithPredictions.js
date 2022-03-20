import { act } from 'react-dom/test-utils';
import '../../Styling/BracketCSS.css'
import Team from './Team'

let predictionWrong = (actualTeam, predictedTeam) => {
    if(!actualTeam || !predictedTeam) return false;
    return actualTeam.id != predictedTeam.id;
}


function TeamWithPredictions({predictedTeam, actualTeam, renderSeed, handleTeamClicked, isLeft}) {
    let makeTeamComponent = (team, isWrong) => {
        let props = { ...team, renderSeed: renderSeed, handleTeamClicked: handleTeamClicked, isLeft, asWrong:isWrong };
        return <Team {...props} />
    }
    let wrong = predictionWrong(actualTeam, predictedTeam);
    if(!wrong) {
        return makeTeamComponent(predictedTeam);
    }
    // team prediction was wrong.
    
    return (
        <div>
            
            {makeTeamComponent(actualTeam)}
            {makeTeamComponent(predictedTeam, true)}
        </div>
    )
    
}

export default TeamWithPredictions;