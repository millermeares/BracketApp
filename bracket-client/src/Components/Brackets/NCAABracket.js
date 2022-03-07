import Game from './Game'
import Team from './Game'
import '../../Styling/BracketCSS.css'
import FinalGame from './FinalGame'
import {v4 as uuid} from 'uuid'
import Finalist from './Finalist'
import {useState} from 'react'


function gamesOfDepth(parentGame, depth) {
    if(depth == 0) {
        return [parentGame];
    }
    let children = [];
    // left games being first actually matters.
    children.push(...gamesOfDepth(parentGame.leftGame, depth-1));
    children.push(...gamesOfDepth(parentGame.rightGame, depth-1));
    return children;
}

function gameOfDepthExists(championshipGame, depth) {
    let game = championshipGame;
    // at depth of 1, i check left game. 
    // at depth of 2, i go to left game, then check left game.
    while(depth > 0) {
        game = game.leftGame;
        if(!game) {
            return false;
        }
        depth--;
    }
    return true;
}

function gamesByDepth(championshipGame) {
    let depth = 1;
    let obj = {};
    while(gameOfDepthExists(championshipGame, depth)) {
        let depth_games = gamesOfDepth(championshipGame, depth);
        obj[depth] = depth_games;
        depth++;
    }
    return obj;
}

function getParentGame(parentGame, id) {
    if(parentGame == null) return null;
    if(!parentGame.leftGame || !parentGame.rightGame) return null;
    if(parentGame.leftGame.id == id || parentGame.rightGame.id == id) return parentGame;
    let child_result = getParentGame(parentGame.leftGame, id);
    if(child_result == null) {
        child_result = getParentGame(parentGame.rightGame, id);
    }
    return child_result;
}


function getTeamInGameMatchingId(game, id) {
    if(game.competitor1.id == id) return game.competitor1;
    if(game.competitor2.id == id) return game.competitor2;
    return null;
}

function handleRemoveNonWinners(champGame, gameId) {
    let parentGame = getParentGame(champGame, gameId);
    console.log("no parent game");
    if (!parentGame) return;
    if (parentGame.competitor1 != parentGame.leftGame.competitor1 && parentGame.competitor1 != parentGame.leftGame.competitor2) {
        console.log("removing competitor 1");
        parentGame.competitor1 = null;
    }
    if (parentGame.competitor2 != parentGame.rightGame.competitor1 && parentGame.competitor2 != parentGame.rightGame.competitor2) {
        console.log("removing competitor 2");
        parentGame.competitor2 = null;
    }
    handleRemoveNonWinners(champGame, parentGame.id);

}


function NCAABracket({ id, name, eventStart, eventEnd, championshipGame }) {
    const [champGame, setChampionshipGame] = useState(championshipGame)
    let handleSetWinner = (gameId, winnerId) => {
        let parentGame = getParentGame(champGame, gameId);
        let winner_is_top = parentGame.leftGame.id == gameId;
        if(winner_is_top) {
            let team_to_set_as_competitor = getTeamInGameMatchingId(parentGame.leftGame, winnerId);
            if(team_to_set_as_competitor == parentGame.competitor1) {
                console.log("winner already top");
                return;
            }
            parentGame.competitor1 = team_to_set_as_competitor;
        } else {
            let team_to_set_as_competitor = getTeamInGameMatchingId(parentGame.rightGame, winnerId);
            if(team_to_set_as_competitor == parentGame.competitor2) {
                console.log("winner already bottom");
                return;
            }
            parentGame.competitor2 = team_to_set_as_competitor;
        }
        console.log("this is parent gmae. now removing non-winners");
        console.log(parentGame);
        handleRemoveNonWinners(champGame, gameId);
        setChampionshipGame({...champGame});
    }


    let games_by_depth = gamesByDepth(champGame);
    let semi_finals_games = games_by_depth[1];
    let elite_eight_games = games_by_depth[2];
    let sweet_16_games = games_by_depth[3];
    let r_o_32_games = games_by_depth[4];
    let seed_games = games_by_depth[5];
    
    let spacerComponent = () => {
        return <li className='spacer' key={uuid()}>&nbsp;</li>
    }

    let makeRoundComponents = (games, left, renderSeed, in_between_spacer, in_between_is_regional) => {
        let game_side_class = left ? "game-left" : "game-right";
        let components = [];
        let in_between_style = game_side_class + " spacer";
        if(in_between_is_regional) {
            in_between_style = in_between_style + " region region-" + (left ? "left" : "right");
        }
        let inBetweenComponent = in_between_spacer ? <li className={in_between_style}>&nbsp;</li> : null;
        for(let i = 0; i < games.length; i++) {
            components.push(spacerComponent())
            let props = {...games[i], className: game_side_class, inBetweenComponent: inBetweenComponent, renderSeed: renderSeed, key: games[i].id, handleSetWinner: handleSetWinner};
            components.push(<Game {...props} />)
        }
        components.push(spacerComponent())
        return components;
    }

    function secondHalf(games) {
        return games.slice(-(games.length / 2));
    }

    function firstHalf(games) {
        return games.slice(0, games.length / 2);
    }

    let makeFinalist1 = (team) => {
        let props = {...team, className: "round semi-final", nameClass: "game-left game-top"}
        return <Finalist {...props}/>
    }

    let makeFinalist2 = (team) => {
        let props = {...team, className: "round semi-final", nameClass: "game-right game-top"}
        return <Finalist {...props}/>
    }

    let makeWinner = (team) => {
        let props = {...team, className: "round finals", nameClass: "game final"}
        return <Finalist {...props}/>
    }
    

    return (
        <div className="tournament">
            <ul className='round seed'>
                {makeRoundComponents(firstHalf(seed_games), true, true, false)}
            </ul>

            <ul className='round round-1'>
                {makeRoundComponents(firstHalf(r_o_32_games), true, false, true)}
            </ul>

            <ul className='round round-2'>
                {makeRoundComponents(firstHalf(sweet_16_games), true, false, true)}
            </ul>

            <ul className='round round-3'>
                {makeRoundComponents(firstHalf(elite_eight_games), true, false, true, true)}
            </ul>

            <ul className='round round-4'>
                {makeRoundComponents(firstHalf(semi_finals_games), true, false, true)}
            </ul>

            {makeFinalist1(champGame.leftGame)}
            {makeWinner(champGame.winner)}
            {makeFinalist2(champGame.rightGame)}

            <ul className='round round-4'>
                {makeRoundComponents(secondHalf(semi_finals_games), true, false, true)}
            </ul>
            <ul className='round round-3'>
                {makeRoundComponents(secondHalf(elite_eight_games), true, false, true, true)}
            </ul>
            <ul className='round round-2'>
                {makeRoundComponents(secondHalf(sweet_16_games), true, false, true)}
            </ul>
            <ul className='round round-1'>
                {makeRoundComponents(secondHalf(r_o_32_games), true, false, true)}
            </ul>
            <ul className='round seed'>
                {makeRoundComponents(secondHalf(seed_games), true, true, false)}
            </ul>

        </div>
    )
    
    
}

export default NCAABracket;