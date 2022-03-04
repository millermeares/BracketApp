import {useState} from 'react';
makeTeamComponent = (team, handleSetWinner) => {
    // ok what is teh best way to pass props ya know?
    return <Team props={...team} />
}


function Game({division, round, team1, team2, winner}) {
    // data structure:
    // round
    // division
    // competitor1
    // competitor2
    // winner

    [componentWinner, setWinner] = useState(winner);

    handleSetWinner = (team) => {
        setWinner(team);
    }
    // use the round, division to style. need to change the winner on team click. 
    // so that's important. 

    let team_1_component = makeTeamComponent(team1);
    let team_2_component = makeTeamComponent(team2);
    return (
        <div className="game">
            <div className="top_game">{team_1_component}</div>
            <div className="bottom_game">{team_2_component}</div>
        </div>
    )
}



export default Game;