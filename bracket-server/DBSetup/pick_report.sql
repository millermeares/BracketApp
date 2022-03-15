SET @tournamentID="05501964-c6da-4542-bee3-7edd307ed575";
SELECT c.competitorName, g._fk_tournamentRound, COUNT(bgp._fk_game) AS 'appearances' FROM tournament t
JOIN competitor_tournament c ON c._fk_tournament=t.tournamentID
LEFT OUTER JOIN bracket_game_prediction bgp ON bgp._fk_tournament=c._fk_tournament AND c.competitorID=bgp._fk_competitor
LEFT OUTER JOIN tournament_game g ON g.gameID=bgp._fk_game AND g._fk_tournament=c._fk_tournament
WHERE t.tournamentID=@tournamentID
GROUP BY c.competitorID, g._fk_tournamentRound 
ORDER BY competitorName, _fk_tournamentRound;

select * from tournament;
set @userID="cf79a314-e3f6-4448-833a-29d6984c311d";
SELECT c.competitorName, c.competitorID, c._fk_division, c._fk_seed, c._fk_tournament, g._fk_tournamentRound, COUNT(bgp._fk_game) AS 'appearances' FROM tournament t
        JOIN competitor_tournament c ON c._fk_tournament=t.tournamentID
        LEFT OUTER JOIN bracket_game_prediction bgp ON bgp._fk_tournament=c._fk_tournament AND c.competitorID=bgp._fk_competitor
        LEFT OUTER JOIN user_bracket b ON b.bracketID=bgp._fk_bracket AND b.completed IS TRUE
        LEFT OUTER JOIN tournament_game g ON g.gameID=bgp._fk_game AND g._fk_tournament=c._fk_tournament
        WHERE t.tournamentID=@tournamentID AND b._fk_user=@userID
        GROUP BY c.competitorID, g._fk_tournamentRound 
        ORDER BY competitorName, _fk_tournamentRound;
        
        
SELECT r.round, r.name 
FROM tournament_round r
JOIN tournament t ON t._fk_type=_fk_tournamentType
WHERE t.tournamentID=@tournamentID;


SELECT tournamentID, name, t.createdTime, creator, t._fk_type AS tournamentType, tournamentFinalized, b._fk_user AS userID, b.completionTime,
	        g._fk_division AS gameDivision, _fk_tournamentRound, gameID, _fk_leftGame, _fk_rightGame, g._fk_competitor_Winner,
	        c._fk_seed, c.competitorName, c.competitorID, c._fk_division AS competitorDivision, 
            bgp._fk_competitor AS winnerPick, bgp._fk_game AS gamePick, b.bracketID, b.completed, b.champTotalPoints, b.creationTime
        FROM user_bracket b 
        JOIN tournament t ON t.tournamentID = b._fk_tournament
        JOIN tournament_game g ON g._fk_tournament=t.tournamentID
        LEFT OUTER JOIN game_participant p ON p._fk_game=g.gameID
        LEFT OUTER JOIN competitor_tournament c ON c.competitorID=p._fk_competitor AND t.tournamentID=c._fk_tournament
        LEFT OUTER JOIN bracket_game_prediction bgp ON g.gameID=bgp._fk_game AND bgp._fk_bracket=b.bracketID
        WHERE b.bracketID=
        ORDER BY g._fk_tournamentRound, gameDivision, g.gameID;