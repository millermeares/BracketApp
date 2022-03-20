import {useState} from 'react';
import Team from './Team';
import '../../Styling/BracketCSS.css'
import TeamWithPredictions from './TeamWithPredictions';

function Game({predictedCompetitor1, predictedCompetitor2, competitor1, competitor2, competitor1PredictionWrong, competitor2PredictionWrong, 
    id, leftGame, rightGame, 
    className, inBetweenComponent, renderSeed, handleSetWinner, isLeft}) {
    // for depth: 
    // 1 is final 4
    // 2 is elite 8
    // 3 is sweet 16
    // 4 is round of 32
    // 5 is seed round. 
    // use this to determine the amount of spaces, etc.
    let handleTeamClicked = (teamId) => {
        handleSetWinner(id, teamId);
    }
    // this syntax might cause nullable issues.
    let teamComponent = (predictedTeam, actualTeam, predictedWrong) => {
        if(!predictedTeam) {
            return singleTeamComponent(actualTeam);
        }
        if(!actualTeam) {
            return singleTeamComponent(predictedTeam, predictedWrong);
        }
        let props = {predictedTeam: {...predictedTeam}, actualTeam: {...actualTeam}, renderSeed: renderSeed, handleTeamClicked: handleTeamClicked, isLeft};
        return <TeamWithPredictions {...props} />
    }

    let singleTeamComponent = (team, asWrong) => {
        let props = {...team, renderSeed: renderSeed, handleTeamClicked: handleTeamClicked ,isLeft, asWrong: asWrong};
        return <Team {...props} />
    }

    
    
    // use the round, division to style. need to change the winner on team click. 
    // so that's important. 

    // this first li needs to have a game-left or game-right tag. but, for now, i'm going to try to render that in Bracket.
    // passing the inbetweencomponent if necessary.
    // here's what i'm learning. the in between component is not a viable strategy. 
    // wait maybe. the in between component actually shouldn't affect the horizontal spacing much. 
    // right? i think so
    
    return (
        <li className={className}>
            <ul className="game-wrapper">
                <li className="game-top">
                    {teamComponent(predictedCompetitor1, competitor1, competitor1PredictionWrong)}
                </li>
                {inBetweenComponent} 
                <li className="game-bottom">
                    {teamComponent(predictedCompetitor2, competitor2, competitor2PredictionWrong)}
                </li>
            </ul>
        </li>
    )
}



export default Game;