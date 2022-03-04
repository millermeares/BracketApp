import {useState} from 'react';
import Team from './Team';
const makeTeamComponent = (team, handleSetWinner) => {
    // ok what is teh best way to pass props ya know?
    return <Team {...team} />
}


function Game({competitor1, competitor2, id, leftGame, rightGame, winner}) {
    // data structure:
    // round
    // division
    // competitor1
    // competitor2
    // winner
    
    
    
    // use the round, division to style. need to change the winner on team click. 
    // so that's important. 

    let team_1_component = makeTeamComponent(competitor1);
    let team_2_component = makeTeamComponent(competitor2);
    return (
        <div className="game">
            <div className="top_game">{team_1_component}</div>
            <div className="bottom_game">{team_2_component}</div>
        </div>
    )
}



export default Game;