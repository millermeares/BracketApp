SET @tournamentID="05501964-c6da-4542-bee3-7edd307ed575";
SELECT c.competitorName, g._fk_tournamentRound, COUNT(bgp._fk_game) AS 'appearances' FROM tournament t
JOIN competitor_tournament c ON c._fk_tournament=t.tournamentID
LEFT OUTER JOIN bracket_game_prediction bgp ON bgp._fk_tournament=c._fk_tournament AND c.competitorID=bgp._fk_competitor
LEFT OUTER JOIN tournament_game g ON g.gameID=bgp._fk_game AND g._fk_tournament=c._fk_tournament
WHERE t.tournamentID=@tournamentID
GROUP BY c.competitorID, g._fk_tournamentRound 
ORDER BY competitorName, _fk_tournamentRound;