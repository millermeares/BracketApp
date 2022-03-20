import Team from './Team';
import TeamWithPredictions from './TeamWithPredictions';
function Finalist({ actualTeam, predictedTeam, predictionWrong, className, nameClass, handleSetWinner }) {
    let makeFinalistSkeleton = (child) => {
        return (
            <ul className={className}>
                <li className='spacer'>&nbsp;</li>
                <li className={nameClass}>
                    {child}
                </li>
                <li className='spacer'>&nbsp;</li>
            </ul>
        )
    }
    let singleTeamComponent = (team, asWrong) => {
        let props = {...team, handleTeamClicked: (handleSetWinner ? handleSetWinner : () => {}), asWrong: asWrong};
        return makeFinalistSkeleton(<Team {...props} />)
    }
    
    if(!predictedTeam) {
        return singleTeamComponent(actualTeam);
    }
    if(!actualTeam || Object.keys(actualTeam).length == 0) {
        return singleTeamComponent(predictedTeam, predictionWrong);
    }
    console.log(actualTeam);

    let prediction_props = {predictedTeam: {...predictedTeam}, actualTeam: {...actualTeam}, handleTeamClicked: handleSetWinner};
    // 
    return (
        <ul className={className}>
            <li className='spacer'>&nbsp;</li>
            <li className={nameClass}>
                
                <TeamWithPredictions {...prediction_props} />
            </li>
            <li className='spacer'>&nbsp;</li>
        </ul>
    )
}

export default Finalist