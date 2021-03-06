import Game from './Game'
import Team from './Game'
import '../../Styling/BracketCSS.css'
import FinalGame from './FinalGame'
import { v4 as uuid } from 'uuid'
import Finalist from './Finalist'
import { useState } from 'react'


function gamesOfDepth(parentGame, depth) {
    if (depth == 0) {
        return [parentGame];
    }
    let children = [];
    // left games being first actually matters.
    children.push(...gamesOfDepth(parentGame.leftGame, depth - 1));
    children.push(...gamesOfDepth(parentGame.rightGame, depth - 1));
    return children;
}

function gameOfDepthExists(championshipGame, depth) {
    let game = championshipGame;
    // at depth of 1, i check left game. 
    // at depth of 2, i go to left game, then check left game.
    while (depth > 0) {
        game = game.leftGame;
        if (!game) {
            return false;
        }
        depth--;
    }
    return true;
}

function gamesByDepth(championshipGame) {
    let depth = 1;
    let obj = {};
    while (gameOfDepthExists(championshipGame, depth)) {
        let depth_games = gamesOfDepth(championshipGame, depth);
        obj[depth] = depth_games;
        depth++;
    }
    return obj;
}

function getParentGame(parentGame, id) {
    if (parentGame == null) return null;
    if (!parentGame.leftGame || !parentGame.rightGame) return null;
    if (parentGame.leftGame.id == id || parentGame.rightGame.id == id) return parentGame;
    let child_result = getParentGame(parentGame.leftGame, id);
    if (child_result == null) {
        child_result = getParentGame(parentGame.rightGame, id);
    }
    return child_result;
}


function getTeamInGameMatchingId(game, id) {
    if (game.predictedCompetitor1 != null && game.predictedCompetitor1.id == id) return game.predictedCompetitor1;
    if (game.predictedCompetitor2 != null && game.predictedCompetitor2.id == id) return game.predictedCompetitor2;
    return null;
}

function handleRemoveNonWinners(champGame, gameId) {
    let parentGame = getParentGame(champGame, gameId);
    if (!parentGame) return;
    if (!compareCompetitors(parentGame.predictedCompetitor1,parentGame.leftGame.predictedCompetitor1) && 
            !compareCompetitors(parentGame.predictedCompetitor1, parentGame.leftGame.predictedCompetitor2)) {
        parentGame.predictedCompetitor1 = null;
    }
    if (!compareCompetitors(parentGame.predictedCompetitor2,parentGame.rightGame.predictedCompetitor1) && 
        !compareCompetitors(parentGame.predictedCompetitor2, parentGame.rightGame.predictedCompetitor2)) {
        parentGame.predictedCompetitor2 = null;
    }
    handleRemoveNonWinners(champGame, parentGame.id);
}

function compareCompetitors(comp1, comp2) {
    if(comp1 == comp2) return true; //same reference, ez
    if(!comp1 || !comp2) return false; // they are both non-null.
    return comp1.id == comp2.id;
}


function NCAABracket({ id, name, eventStart, eventEnd, championshipGame, beforeSetWinnerPromise }) {
    const [champGame, setChampionshipGame] = useState(championshipGame)
    let handleSetWinner = (gameId, winnerId) => {
        beforeSetWinnerPromise(gameId, winnerId).then(updateUI => {
            if(!updateUI) return; // if not success, i don't want it to affect anything.
            let parentGame = getParentGame(champGame, gameId);
            let winner_is_top = parentGame.leftGame.id == gameId;
            if (winner_is_top) {
                let team_to_set_as_competitor = getTeamInGameMatchingId(parentGame.leftGame, winnerId);
                if (compareCompetitors(team_to_set_as_competitor, parentGame.predictedCompetitor1)) {
                    return;
                }
                if (compareCompetitors(parentGame.predictedCompetitor1,champGame.predictedWinner)) {
                    champGame.predictedWinner = null; // handle clearing winner too.
                }
                parentGame.predictedCompetitor1 = team_to_set_as_competitor;
            } else {
                let team_to_set_as_competitor = getTeamInGameMatchingId(parentGame.rightGame, winnerId);
                if (compareCompetitors(team_to_set_as_competitor,parentGame.predictedCompetitor2)) {
                    return;
                }
                if (compareCompetitors(parentGame.predictedDompetitor2, champGame.predictedWinner)) {
                    champGame.predictedWinner = null; // handle clearing winner too.
                }
                parentGame.predictedCompetitor2 = team_to_set_as_competitor;
            }
            handleRemoveNonWinners(champGame, gameId);
            setChampionshipGame({ ...champGame });
        }).catch(err => {
            console.log(err);
        })
    }
    let sending_champ_request = false;
    let handleSetChampWinner = (winnerId) => {
        if(sending_champ_request) return;
        sending_champ_request = true;
        beforeSetWinnerPromise(champGame.id, winnerId).then(success => {
            sending_champ_request = false;
            if(!success){
                return;
            }
            champGame.predictedWinner = getTeamInGameMatchingId(champGame, winnerId);
        setChampionshipGame({...champGame});
        }).catch(err => {
            sending_champ_request = false;
            console.log(err);
        });
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
            let props = {...games[i], className: game_side_class, inBetweenComponent: inBetweenComponent, renderSeed: renderSeed, 
                key: games[i].id, 
                handleSetWinner: handleSetWinner, 
                isLeft: left};
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

    let makeFinalist1 = (predictedTeam, actualTeam, predictionWrong) => {
        let props = {predictedTeam:{...predictedTeam}, actualTeam: {...actualTeam}, predictionWrong:predictionWrong, 
            className: "round semi-final", nameClass: "game-left game-top", handleSetWinner: handleSetChampWinner}
        return <Finalist {...props}/>
    }

    let makeFinalist2 = (predictedTeam, actualTeam, predictionWrong) => {
        let props = {predictedTeam:{...predictedTeam}, actualTeam: {...actualTeam}, predictionWrong:predictionWrong, 
            className: "round semi-final", nameClass: "game-right game-top", handleSetWinner: handleSetChampWinner}
        return <Finalist {...props}/>
    }

    let makeWinner = (predictedTeam, actualTeam, predictionWrong) => {
        let props = {predictedTeam:{...predictedTeam}, actualTeam: {...actualTeam}, predictionWrong:predictionWrong, 
        className: "round finals", nameClass: "game final"}
        return <Finalist {...props}/>
    }

    let predicted_champ_already_lost = () => {
        if(compareCompetitors(champGame.predictedWinner, champGame.predictedCompetitor1)) {
            return champGame.competitor1PredictionWrong;
        }
        if(compareCompetitors(champGame.predictedWinner, champGame.predictedCompetitor2)) {
            return champGame.competitor2PredictionWrong;
        }
        return false;
    }

    return (
        <div className="tournament">
            <ul className='round seed'>
                {makeRoundComponents(firstHalf(seed_games), true, true, false)}
            </ul>

            <ul className='round round-1'>
                {makeRoundComponents(firstHalf(r_o_32_games), true, true, true)}
            </ul>

            <ul className='round round-2'>
                {makeRoundComponents(firstHalf(sweet_16_games), true, true, true)}
            </ul>

            <ul className='round round-3'>
                {makeRoundComponents(firstHalf(elite_eight_games), true, true, true, true)}
            </ul>

            <ul className='round round-4'>
                {makeRoundComponents(firstHalf(semi_finals_games), true, true, true)}
            </ul>

            {makeFinalist1(champGame.predictedCompetitor1, champGame.competitor1, champGame.competitor1PredictionWrong)}
            {makeWinner(champGame.predictedWinner, champGame.winner, predicted_champ_already_lost())}
            {makeFinalist2(champGame.predictedCompetitor2, champGame.competitor2, champGame.competitor2PredictionWrong)}

            <ul className='round round-4'>
                {makeRoundComponents(secondHalf(semi_finals_games), false, true, true)}
            </ul>
            <ul className='round round-3'>
                {makeRoundComponents(secondHalf(elite_eight_games), false, true, true, true)}
            </ul>
            <ul className='round round-2'>
                {makeRoundComponents(secondHalf(sweet_16_games), false, true, true)}
            </ul>
            <ul className='round round-1'>
                {makeRoundComponents(secondHalf(r_o_32_games), false, true, true)}
            </ul>
            <ul className='round seed'>
                {makeRoundComponents(secondHalf(seed_games), false, true, false)}
            </ul>
        </div>
    )
}

export default NCAABracket;