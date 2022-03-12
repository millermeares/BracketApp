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
ORDER BY g._fk_tournamentRound, gameDivision, g.gameID

