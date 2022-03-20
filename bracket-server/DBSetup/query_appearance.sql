select * from user;
select * from tournament;
set @userID="094570b5-cff4-4364-84ad-05bcb2d9a7ac";
set @tournamentID="74c2a92a-9e04-4f79-898a-58bba73c7abc";
SELECT c.competitorName, c.competitorID, c._fk_division, c._fk_seed, c._fk_tournament, g._fk_tournamentRound, COUNT(bgp._fk_game) AS 'appearances' FROM tournament t
        JOIN competitor_tournament c ON c._fk_tournament=t.tournamentID
        LEFT OUTER JOIN bracket_game_prediction bgp ON bgp._fk_tournament=c._fk_tournament AND c.competitorID=bgp._fk_competitor
        LEFT OUTER JOIN user_bracket b ON b.bracketID=bgp._fk_bracket AND b.completed IS TRUE
        LEFT OUTER JOIN tournament_game g ON g.gameID=bgp._fk_game AND g._fk_tournament=c._fk_tournament
        WHERE t.tournamentID=@tournamentID AND b._fk_user=@userID
        GROUP BY c.competitorID, g._fk_tournamentRound 
        ORDER BY competitorName, _fk_tournamentRound;