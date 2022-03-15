# get tournament: 
select * from tournament;
select * from tournament_game;

SELECT tournamentID, name, creator, _fk_type, tournamentFinalized, 
	g._fk_division AS gameDivision, _fk_tournamentRound, gameID, _fk_leftGame, _fk_rightGame, 
	c._fk_seed, c.competitorName, c.competitorID, c._fk_division AS competitorDivision
FROM tournament t 
JOIN tournament_game g ON g._fk_tournament=t.tournamentID
LEFT OUTER JOIN game_participant p ON p._fk_game=g.gameID
LEFT OUTER JOIN competitor_tournament c ON c.competitorID=p._fk_competitor AND t.tournamentID=c._fk_tournament
WHERE t.tournamentID=@tournamentID;

# get bracket: 
select count(*) from (
SELECT tournamentID, name, creator, _fk_type, tournamentFinalized, 
	        g._fk_division AS gameDivision, _fk_tournamentRound, gameID, _fk_leftGame, _fk_rightGame, g._fk_competitor_Winner,
	        c._fk_seed, c.competitorName, c.competitorID, c._fk_division AS competitorDivision, 
            bgp._fk_competitor AS winnerPick, bgp._fk_game AS gamePick, b.bracketID, b.completed, b.champTotalPoints, b.creationTime
        FROM user_bracket b 
        JOIN tournament t ON t.tournamentID = b._fk_tournament
        JOIN tournament_game g ON g._fk_tournament=t.tournamentID
        LEFT OUTER JOIN game_participant p ON p._fk_game=g.gameID
        LEFT OUTER JOIN competitor_tournament c ON c.competitorID=p._fk_competitor AND t.tournamentID=c._fk_tournament
        LEFT OUTER JOIN bracket_game_prediction bgp ON g.gameID=bgp._fk_game AND bgp._fk_bracket=b.bracketID
WHERE b.bracketID=@bracketID
ORDER BY g._fk_tournamentRound, gameDivision, g.gameID
) bracket_query 
where bracket_query._fk_tournamentRound=2;

set @bracketID="b29d36e7-b5c9-41dc-affa-207d3f6abbbf";
select * from user_bracket;
select count(*) from user_bracket WHERE _fk_tournamentRound=1;

delete from user_bracket where bracketID="bracket_server.Brackets.NewBracket";


select * from tournament_game order by _fk_tournamentRound, _fk_division, gameID;

SELECT tournamentID, name, creator, _fk_type, tournamentFinalized, 
	        g._fk_division AS gameDivision, _fk_tournamentRound, gameID, _fk_leftGame, _fk_rightGame, g._fk_competitor_Winner,
	        c._fk_seed, c.competitorName, c.competitorID, c._fk_division AS competitorDivision, 
            bgp._fk_competitor AS winnerPick, bgp._fk_game AS gamePick, b.bracketID, b.completed, b.champTotalPoints, b.creationTime
        FROM user_bracket b 
        JOIN tournament t ON t.tournamentID = b._fk_tournament
        JOIN tournament_game g ON g._fk_tournament=t.tournamentID
        LEFT OUTER JOIN game_participant p ON p._fk_game=g.gameID
        LEFT OUTER JOIN competitor_tournament c ON c.competitorID=p._fk_competitor AND t.tournamentID=c._fk_tournament
        LEFT OUTER JOIN bracket_game_prediction bgp ON g.gameID=bgp._fk_game AND bgp._fk_bracket=b.bracketID
WHERE b.bracketID=@bracketID
ORDER BY g._fk_tournamentRound, gameDivision, g.gameID;



select * from tournament_game;

SELECT b.bracketID, b.creationTime, b.completionTime, t.name AS tournamentName,competitorName   FROM user_bracket b
JOIN tournament t ON t.tournamentID=b._fk_tournament
JOIN (
	SELECT MAX(round) AS champRound, _fk_tournamentType FROM tournament_round GROUP BY _fk_tournamentType
) maxRound ON maxRound._fk_tournamentType=t._fk_type
JOIN tournament_game g ON t.tournamentID=g._fk_tournament AND g._fk_tournamentRound=maxRound.champRound
JOIN bracket_game_prediction bgp ON b.bracketID=bgp._fk_bracket AND g.gameID=bgp._fk_game
JOIN competitor_tournament c ON c._fk_tournament=t.tournamentID AND c.competitorID=bgp._fk_competitor
WHERE b.completed IS TRUE AND b._fk_user=@userID
ORDER BY b.completionTime DESC;

SELECT b.bracketID, b.creationTime, b.completionTime, t.name AS tournamentName,competitorName   FROM user_bracket b
        JOIN tournament t ON t.tournamentID=b._fk_tournament
        JOIN (
	        SELECT MAX(round) AS champRound, _fk_tournamentType FROM tournament_round GROUP BY _fk_tournamentType
        ) maxRound ON maxRound._fk_tournamentType=t._fk_type
        JOIN tournament_game g ON t.tournamentID=g._fk_tournament AND g._fk_tournamentRound=maxRound.champRound
        JOIN bracket_game_prediction bgp ON b.bracketID=bgp._fk_bracket AND g.gameID=bgp._fk_game
        JOIN competitor_tournament c ON c._fk_tournament=t.tournamentID AND c.competitorID=bgp._fk_competitor
        WHERE b.completed IS TRUE AND b._fk_user=@userID
        ORDER BY b.completionTime DESC;
select * from user;
set @userID="cf79a314-e3f6-4448-833a-29d6984c311d";
SELECT MAX(_fk_tournamentRound) AS 'champRound', g._fk_tournament AS tournamentID 
FROM tournament_game g
JOIN user_bracket b ON b._fk_tournament=g._fk_tournament
WHERE b._fk_user=@userID AND b.completed IS TRUE
GROUP BY g._fk_tournament;


select * from tournament;
select * from tournament_game;
select * from tournament_division;
delete from competitor_tournament where _fk_tournament="0790a500-41fe-461b-a62e-9c389f1d796a";


SELECT kpm.offensiveefficiency, kpm.defensiveefficiency, kpm.overallefficiency, kpm.tempo, IF(kpm.overallEfficiency IS NULL, FALSE, TRUE) AS 'hasKenPom', c.competitorName, c.competitorID, c._fk_division, c._fk_seed, c._fk_tournament
            FROM competitor_tournament c
            LEFT OUTER JOIN ken_pom_data kpm ON c._fk_tournament=kpm._fk_tournament AND c.competitorID=kpm._fk_competitor
            WHERE c._fk_tournament=@tournamentID;
	select * from ken_pom_data;
    
    
    select * from tournament;