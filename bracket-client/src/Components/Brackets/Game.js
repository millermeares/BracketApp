import {useState} from 'react';
import Team from './Team';
import '../../Styling/BracketCSS.css'


function Game({competitor1, competitor2, id, leftGame, rightGame, className, inBetweenComponent, renderSeed, handleSetWinner}) {
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
    let teamComponent = (team) => {
        let props = {...team, renderSeed: renderSeed, handleTeamClicked: handleTeamClicked};
        return <Team {...props} />
    }

    
    
    // use the round, division to style. need to change the winner on team click. 
    // so that's important. 

    // this first li needs to have a game-left or game-right tag. but, for now, i'm going to try to render that in Bracket.
    // passing the inbetweencomponent if necessary.
    return (
        <li className={className}>
            <ul className="game-wrapper">
                <li className="game-top">
                    {teamComponent(competitor1)}
                </li>
                {inBetweenComponent}
                <li className="game-bottom">
                    {teamComponent(competitor2)}
                </li>
            </ul>

        </li>
    )
}



export default Game;